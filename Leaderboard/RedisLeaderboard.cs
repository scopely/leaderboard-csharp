using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookSleeve;

namespace Leaderboard
{
    public class RedisLeaderboard<T> : BaseLeaderboard<T>, IRedisLeaderboard<T>, IDisposable
    {
        public static string DEFAULT_HOST = "localhost";
        public static int DEFAULT_PORT = 6379;
        public static int DEFAULT_DB = 0;

        public int Db { get; set; }

        private readonly Lazy<RedisConnection> _connection; 
        protected virtual RedisConnection Connection
        {
            get { return _connection.Value; }
        }

        public RedisLeaderboard(string leaderboardName, int pageSize, bool reverse, string host, int port, int db)
            : base(leaderboardName, pageSize, reverse)
        {
            Db = db;
            _connection = new Lazy<RedisConnection>(() =>
                                                        {
                                                            var connection = new RedisConnection(host, port);
                                                            connection.Open();
                                                            return connection;
                                                        });
        }

        public RedisLeaderboard(string leaderboardName, int pageSize, bool reverse)
            : this(leaderboardName, pageSize, reverse, DEFAULT_HOST, DEFAULT_PORT, DEFAULT_DB)
        { }

        public RedisLeaderboard(string leaderboardName) : this(leaderboardName, DEFAULT_PAGE_SIZE, DEFAULT_REVERSE)
        { }

        public override void DeleteLeaderboard(string leaderboardName)
        {
            using (var tran = Connection.CreateTransaction())
            {
                tran.Keys.Remove(Db, leaderboardName);
                tran.Keys.Remove(Db, GetMemberDataKey(leaderboardName));

                Connection.Wait(tran.Execute());
            }
        }

        public override void RankMember(string leaderboardName, string member, double score, T data = default(T))
        {
            using (var tran = Connection.CreateTransaction())
            {
                tran.SortedSets.Add(Db, leaderboardName, member, score);
                if (data != null)
                {
                    tran.Hashes.Set(Db, GetMemberDataKey(leaderboardName), member, data.ToJson());
                }

                Connection.Wait(tran.Execute());
            }
        }

        public override T GetMemberData(string leaderboardName, string member)
        {
            var hg = Connection.Hashes.GetString(Db, GetMemberDataKey(leaderboardName), member);
            var data = Connection.Wait(hg);
            return data.FromJson<T>();
        }

        public override void UpdateMemberData(string leaderboardName, string member, T data)
        {
            var hs = Connection.Hashes.Set(Db, GetMemberDataKey(leaderboardName), member, data.ToJson());
            Connection.Wait(hs);
        }

        public override void RemoveMemberData(string leaderboardName, string member)
        {
            var hd = Connection.Hashes.Remove(Db, GetMemberDataKey(leaderboardName), member);
            Connection.Wait(hd);
        }

        public override void RankMembers(string leaderboardName, IEnumerable<MemberScorePair> memberScores)
        {
            using (var tran = Connection.CreateTransaction())
            {
                foreach (var memberScorePair in memberScores)
                {
                    tran.SortedSets.Add(Db, leaderboardName, memberScorePair.Member, memberScorePair.Score);
                }

                Connection.Wait(tran.Execute());
            }
        }

        public override void RemoveMember(string leaderboardName, string member)
        {
            using (var tran = Connection.CreateTransaction())
            {
                tran.SortedSets.Remove(Db, leaderboardName, member);
                tran.Hashes.Remove(Db, GetMemberDataKey(leaderboardName), member);

                Connection.Wait(tran.Execute());
            }
        }

        public override long TotalMembers(string leaderboardName)
        {
            var zc = Connection.SortedSets.GetLength(Db, leaderboardName);
            return Connection.Wait(zc);
        }

        public override long TotalMembersInScoreRange(string leaderboardName, double minScore, double maxScore)
        {
            var zc = Connection.SortedSets.GetLength(Db, leaderboardName, minScore, maxScore);
            return Connection.Wait(zc);
        }

        public override double ChangeScore(string leaderboardName, string member, double scoreDelta)
        {
            var zi = Connection.SortedSets.Increment(Db, leaderboardName, member, scoreDelta);
            return Connection.Wait(zi);
        }

        public override long? GetRank(string leaderboardName, string member)
        {
            var zr = Connection.SortedSets.Rank(Db, leaderboardName, member, Reverse);
            var rank = Connection.Wait(zr);
            return rank.HasValue ? rank + 1 : null;
        }

        public override double? GetScore(string leaderboardName, string member)
        {
            var zs = Connection.SortedSets.Score(Db, leaderboardName, member);
            return Connection.Wait(zs);
        }

        public override Record<T> GetRecord(string leaderboardName, string member)
        {
            using (var tran = Connection.CreateTransaction())
            {
                var zs = tran.SortedSets.Score(Db, leaderboardName, member);
                var zr = tran.SortedSets.Rank(Db, leaderboardName, member, Reverse);
                var exec = tran.Execute();

                var score = Connection.Wait(zs);
                var rank = Connection.Wait(zr);
                Connection.Wait(exec);

                if (score.HasValue && rank.HasValue)
                {
                    return new Record<T>(member, score.Value, rank.Value);
                }

                return null;
            }
        }

        public override void RemoveMembersInScoreRange(string leaderboardName, double minScore, double maxScore)
        {
            var zr = Connection.SortedSets.RemoveRange(Db, leaderboardName, minScore, maxScore);
            Connection.Wait(zr);
        }

        public override int? GetPercentile(string leaderboardName, string member)
        {
            using (var tran = Connection.CreateTransaction())
            {
                var zc = tran.SortedSets.GetLength(Db, leaderboardName);
                var zr = tran.SortedSets.Rank(Db, leaderboardName, member);
                var exec = tran.Execute();

                var count = Connection.Wait(zc);
                var rank = Connection.Wait(zr);

                int? percentile;
                if (rank == null)
                {
                    percentile = null;
                }
                else
                {
                    percentile = (int) Math.Ceiling((double) rank / count * 100);

                    if (Reverse)
                    {
                        percentile = 100 - percentile;
                    }
                }

                Connection.Wait(exec);
                
                return percentile;
            }
        }

        public override IEnumerable<Record<T>> GetMembers(string leaderboardName, int page, LeaderboardOptions options = null)
        {
            if (page < 1)
            {
                page = 1;
            }

            options = options ?? new LeaderboardOptions();
            var pageSize = ValidatePageSize(options.PageSize) ?? PageSize;

            var totalPages = TotalPages(leaderboardName, pageSize);
            if (page > totalPages)
            {
                page = totalPages;
            }

            var index = page - 1;
            var startingOffset = (index * pageSize);
            if (startingOffset < 0)
            {
                startingOffset = 0;
            }

            var endingOffset = startingOffset + pageSize - 1;

            var zr = Connection.SortedSets.RangeString(Db, leaderboardName, startingOffset, endingOffset, Reverse);
            var items = Connection.Wait(zr);

            if (items != null)
            {
                return GetRankedList(leaderboardName, items.Select(r => r.Key), options);
            }

            return new Record<T>[0];
        }

        public override IEnumerable<Record<T>> GetAllMembers(string leaderboardName, LeaderboardOptions options = null)
        {
            var zr = Connection.SortedSets.RangeString(Db, leaderboardName, 0, -1, Reverse);
            var items = Connection.Wait(zr);

            if (items != null)
            {
                return GetRankedList(leaderboardName, items.Select(r => r.Key), options);
            }

            return new Record<T>[0];
        }

        public override IEnumerable<Record<T>> GetMembersInScoreRange(string leaderboardName, double minScore, double maxScore,
                                                           LeaderboardOptions options = null)
        {
            var zr = Connection.SortedSets.Range(Db, leaderboardName, minScore, maxScore, Reverse);
            var items = Connection.Wait(zr);

            if (items != null)
            {
                return GetRankedList(leaderboardName, items.Select(r => ConvertBytes(r.Key)), options);
            }

            return new Record<T>[0];
        }

        public override IEnumerable<Record<T>> GetMembersInRankRange(string leaderboardName, long startRank, long endRank, LeaderboardOptions options = null)
        {
            startRank -= 1;
            if (startRank < 0)
            {
                startRank = 0;
            }

            var totalMembers = TotalMembers(leaderboardName);
            endRank -= 1;
            if (endRank >= totalMembers)
            {
                endRank = totalMembers - 1;
            }

            var zr = Connection.SortedSets.RangeString(Db, leaderboardName, startRank, endRank, Reverse);
            var items = Connection.Wait(zr);

            if (items != null)
            {
                return GetRankedList(leaderboardName, items.Select(r => r.Key), options);
            }

            return new Record<T>[0];
        }

        public override IEnumerable<Record<T>> GetRankedList(string leaderbaordName, IEnumerable<string> members, LeaderboardOptions options = null)
        {
            IEnumerable<Record<T>> rankedMembers = null;
            var ranks = new List<Task<long?>>();
            var scores = new List<Task<double?>>();
            Task<bool> exec;

            using (var tran = Connection.CreateTransaction())
            {
                foreach (var member in members)
                {
                    ranks.Add(tran.SortedSets.Rank(Db, leaderbaordName, member, Reverse));
                    scores.Add(tran.SortedSets.Score(Db, leaderbaordName, member));
                }

                exec = tran.Execute();
            }

            rankedMembers = members.Select((m, i) =>
                                               {
                                                   var rank = Connection.Wait(ranks[i]);
                                                   var score = Connection.Wait(scores[i]);
                                                   T data = default(T);

                                                   if (rank == null)
                                                   {
                                                       return null;
                                                   }

                                                   if (options.WithMemberData)
                                                   {
                                                       data = GetMemberData(leaderbaordName, m);
                                                   }

                                                   return new Record<T>(m, score.Value, rank.Value + 1, data);
                                               });
            rankedMembers = rankedMembers.Where(r => r != null);

            switch (options.SortBy)
            {
                case SortBy.Rank:
                    rankedMembers = rankedMembers.OrderBy(r => r.Rank);
                    break;
                case SortBy.Score:
                    rankedMembers = rankedMembers.OrderBy(r => r.Score);
                    break;
            }

            Connection.Wait(exec);

            return rankedMembers;
        }

        public virtual void ExpireLeaderboard(int seconds)
        {
            ExpireLeaderboard(LeaderboardName, seconds);
        }

        public virtual void ExpireLeaderboard(string leaderboardName, int seconds)
        {
            var ke = Connection.Keys.Expire(Db, leaderboardName, seconds);
            Connection.Wait(ke);
        }

        public virtual void ExpireLeaderboardAt(DateTime time)
        {
            ExpireLeaderboardAt(LeaderboardName, time);
        }

        public virtual void ExpireLeaderboardAt(string leaderboardName, DateTime time)
        {
            var seconds = (time.ToUniversalTime() - DateTime.UtcNow).TotalSeconds;
            ExpireLeaderboard((int) seconds);
        }

        public virtual void MergeLeaderboards(string destination, IEnumerable<string> keys, Aggregate aggregate = Aggregate.SUM)
        {
            var keyArray = (new[] {LeaderboardName}).Concat(keys);
            var zu = Connection.SortedSets.UnionAndStore(Db, destination, keyArray.ToArray(),
                                                         aggregate.ToRedisAggregate());
            Connection.Wait(zu);
        }

        public virtual void IntersectLeaderboards(string destination, IEnumerable<string> keys, Aggregate aggregate = Aggregate.SUM)
        {
            var keyArray = (new[] { LeaderboardName }).Concat(keys);
            var zu = Connection.SortedSets.IntersectAndStore(Db, destination, keyArray.ToArray(),
                                                             aggregate.ToRedisAggregate());
            Connection.Wait(zu);
        }

        protected virtual string GetMemberDataKey(string leaderboardName)
        {
            return String.Format("{0}:member_data", leaderboardName);
        }

        protected virtual string ConvertBytes(byte[] bytes)
        {
            return bytes == null
                       ? null
                       : bytes.Length == 0
                             ? ""
                             : Encoding.UTF8.GetString(bytes);
        }

        public virtual void Close()
        {
            if (_connection.IsValueCreated)
            {
                _connection.Value.Close(false);
            }
        }

        public virtual void Dispose()
        {
            if (_connection.IsValueCreated)
            {
                _connection.Value.Dispose();
            }
        }
    }
}
