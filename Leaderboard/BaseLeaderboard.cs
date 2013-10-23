using System;
using System.Collections.Generic;
using System.Linq;

namespace Leaderboard
{
    public abstract class BaseLeaderboard<K, V, T> : ILeaderboard<K, V, T> where V: struct
    {
        public static int DEFAULT_PAGE_SIZE = 25;
        public static bool DEFAULT_REVERSE = false;

        public string LeaderboardName { get; set; }
        
        public bool Reverse { get; set; }

        private int _pageSize;
        public int PageSize 
        {
            get { return _pageSize; }
            set { _pageSize = value < 1 ? DEFAULT_PAGE_SIZE : value; } 
        }

        protected BaseLeaderboard(string leaderboardName, int pageSize, bool reverse)
        {
            LeaderboardName = leaderboardName;
            PageSize = pageSize;
            Reverse = reverse;
        }

        protected BaseLeaderboard(string leaderboardName) : this(leaderboardName, DEFAULT_PAGE_SIZE, DEFAULT_REVERSE)
        { }

        public virtual bool CheckLeaderboard()
        {
            return CheckLeaderboard(LeaderboardName);
        }

        public abstract bool CheckLeaderboard(string leaderboardName);

        public virtual void DeleteLeaderboard()
        {
            DeleteLeaderboard(LeaderboardName);
        }

        public abstract void DeleteLeaderboard(string leaderboardName);

        public virtual void RankMember(K member, V score, T data = default(T))
        {
            RankMember(LeaderboardName, member, score);
        }

        public abstract void RankMember(string leaderboardName, K member, V score, T data = default(T));

        public virtual bool RankMemberIf(Func<K, V?, V, T, bool, bool> condition, K member, V score,
                                 T data = default(T))
        {
            return RankMemberIf(LeaderboardName, condition, member, score, data);
        }

        public virtual bool RankMemberIf(string leaderboardName, Func<K, V?, V, T, bool, bool> condition,
                                         K member, V score, T data = default(T))
        {
            V? currScore = GetScore(leaderboardName, member);

            if (condition(member, currScore, score, data, Reverse))
            {
                RankMember(leaderboardName, member, score, data);
                return true;
            }

            return false;
        }

        public virtual T GetMemberData(K member)
        {
            return GetMemberData(LeaderboardName, member);
        }

        public abstract T GetMemberData(string leaderboardName, K member);

        public virtual void UpdateMemberData(K member, T data)
        {
            UpdateMemberData(LeaderboardName, member, data);
        }

        public abstract void UpdateMemberData(string leaderboardName, K member, T data);

        public virtual void RemoveMemberData(K member)
        {
            RemoveMember(LeaderboardName, member);
        }

        public abstract void RemoveMemberData(string leaderboardName, K member);

        public virtual void RankMembers(IEnumerable<MemberScorePair<K, V>> memberScores)
        {
            RankMembers(LeaderboardName, memberScores);
        }

        public abstract void RankMembers(string leaderboardName, IEnumerable<MemberScorePair<K, V>> memberScores);

        public virtual void RemoveMember(K member)
        {
            RemoveMember(LeaderboardName, member);
        }

        public abstract void RemoveMember(string leaderboardName, K member);

        public virtual long TotalMembers()
        {
            return TotalMembers(LeaderboardName);
        }

        public abstract long TotalMembers(string leaderboardName);

        public virtual int TotalPages(int? pageSize = null)
        {
            return TotalPages(LeaderboardName, pageSize);
        }

        public virtual int TotalPages(string leaderboardName, int? pageSize = null)
        {
            pageSize = ValidatePageSize(pageSize) ?? PageSize;
            return (int) Math.Ceiling((double) TotalMembers(leaderboardName) / pageSize.Value);
        }

        public virtual long TotalMembersInScoreRange(V minScore, V maxScore)
        {
            return TotalMembersInScoreRange(LeaderboardName, minScore, maxScore);
        }

        public abstract long TotalMembersInScoreRange(string leaderboardName, V minScore, V maxScore);

        public virtual V ChangeScore(K member, V scoreDelta)
        {
            return ChangeScore(LeaderboardName, member, scoreDelta);
        }

        public abstract V ChangeScore(string leaderboardName, K member, V scoreDelta);

        public virtual long? GetRank(K member)
        {
            return GetRank(LeaderboardName, member);
        }

        public abstract long? GetRank(string leaderboardName, K member);

        public virtual V? GetScore(K member)
        {
            return GetScore(LeaderboardName, member);
        }

        public abstract V? GetScore(string leaderboardName, K member);

        public virtual bool CheckMember(K member)
        {
            return CheckMember(LeaderboardName, member);
        }

        public virtual bool CheckMember(string leaderboardName, K member)
        {
            var score = GetScore(leaderboardName, member);
            return score.HasValue;
        }

        public virtual Record<K,V, T> GetRecord(K member)
        {
            return GetRecord(LeaderboardName, member);
        }

        public abstract Record<K, V, T> GetRecord(string leaderboardName, K member);

        public virtual void RemoveMembersInScoreRange(V minScore, V maxScore)
        {
            RemoveMembersInScoreRange(LeaderboardName, minScore, maxScore);
        }

        public abstract void RemoveMembersInScoreRange(string leaderboardName, V minScore, V maxScore);

        public virtual int? GetPercentile(K member)
        {
            return GetPercentile(LeaderboardName, member);
        }

        public abstract int? GetPercentile(string leaderboardName, K member);

        public virtual int GetPage(K member, int? pageSize = null)
        {
            return GetPage(LeaderboardName, member, pageSize);
        }

        public virtual int GetPage(string leaderboardName, K member, int? pageSize = null)
        {
            var rank = GetRank(leaderboardName, member);
            pageSize = ValidatePageSize(pageSize) ?? DEFAULT_PAGE_SIZE;

            if (rank == null)
            {
                rank = 0;
            }

            return (int) Math.Ceiling((double) rank.Value / pageSize.Value);
        }

        public virtual IEnumerable<Record<K, V, T>> GetMembers(int page, LeaderboardOptions options = null)
        {
            return GetMembers(LeaderboardName, page, options);
        }

        public abstract IEnumerable<Record<K, V, T>> GetMembers(string leaderboardName, int page,
                                                                LeaderboardOptions options = null);

        public virtual IEnumerable<Record<K, V, T>> GetAllMembers(LeaderboardOptions options = null)
        {
            return GetAllMembers(LeaderboardName, options);
        }

        public abstract IEnumerable<Record<K, V, T>> GetAllMembers(string leaderboardName, LeaderboardOptions options = null);

        public virtual IEnumerable<Record<K, V, T>> GetMembersInScoreRange(V minScore, V maxScore,
                                                                           LeaderboardOptions options = null)
        {
            return GetMembersInScoreRange(LeaderboardName, minScore, maxScore, options);
        }

        public abstract IEnumerable<Record<K, V, T>> GetMembersInScoreRange(string leaderboardName, V minScore,
                                                                            V maxScore,
                                                                            LeaderboardOptions options = null);

        public virtual IEnumerable<Record<K, V, T>> GetMembersInRankRange(long startRank, long endRank,
                                                                          LeaderboardOptions options = null)
        {
            return GetMembersInRankRange(LeaderboardName, startRank, endRank, options);
        }

        public abstract IEnumerable<Record<K, V, T>> GetMembersInRankRange(string leaderboardName, long startRank,
                                                                           long endRank,
                                                                           LeaderboardOptions options = null);

        public virtual Record<K, V, T> GetMemberAt(long position, LeaderboardOptions options = null)
        {
            return GetMemberAt(LeaderboardName, position, options);
        }

        public virtual Record<K, V, T> GetMemberAt(string leaderboardName, long position,
                                                   LeaderboardOptions options = null)
        {
            if (position <= TotalMembers(LeaderboardName))
            {
                options = options ?? new LeaderboardOptions();
                var pageSize = ValidatePageSize(options.PageSize) ?? PageSize;
                var currPage = (int) Math.Ceiling((double) position / pageSize);
                var offset = (int) ((position - 1) % pageSize);

                var members = GetMembers(leaderboardName, currPage, options);
                if (members == null)
                {
                    return null;
                }

                var membersList = members as IList<Record<K, V, T>> ?? members.ToList();
                if (membersList.Any())
                {
                    return membersList[offset];
                }
            }

            return null;
        }

        public virtual IEnumerable<Record<K, V, T>> GetAroundMe(K member, LeaderboardOptions options = null)
        {
            return GetAroundMe(LeaderboardName, member, options);
        }

        public virtual IEnumerable<Record<K, V, T>> GetAroundMe(string leaderboardName, K member,
                                                                LeaderboardOptions options = null)
        {
            var reverseRank = GetRank(leaderboardName, member);
            if (reverseRank == null)
            {
                return new Record<K, V, T>[0];
            }

            options = options ?? new LeaderboardOptions();
            var pageSize = ValidatePageSize(options.PageSize) ?? PageSize;

            var startRank = reverseRank.Value - (pageSize / 2);
            if (startRank < 0)
            {
                startRank = 0;
            }

            var endRank = startRank + pageSize - 1;

            return GetMembersInRankRange(leaderboardName, startRank, endRank, options);
        }

        public virtual IEnumerable<Record<K, V, T>> GetRankedList(IEnumerable<K> members,
                                                                  LeaderboardOptions options = null)
        {
            return GetRankedList(LeaderboardName, members, options);
        }

        public abstract IEnumerable<Record<K, V, T>> GetRankedList(string leaderbaordName, IEnumerable<K> members,
                                                                   LeaderboardOptions options = null);

        protected int? ValidatePageSize(int? pageSize)
        {
            if (pageSize != null && pageSize < 1)
            {
                pageSize = DEFAULT_PAGE_SIZE;
            }

            return pageSize;
        }
    }
}
