using System;
using System.Collections.Generic;
using System.Linq;

namespace Leaderboard
{
    public abstract class BaseLeaderboard<T> : ILeaderboard<T>
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

        public virtual void DeleteLeaderboard()
        {
            DeleteLeaderboard(LeaderboardName);
        }

        public abstract void DeleteLeaderboard(string leaderboardName);

        public virtual void RankMember(string member, double score, T data = default(T))
        {
            RankMember(LeaderboardName, member, score);
        }

        public abstract void RankMember(string leaderboardName, string member, double score, T data = default(T));

        public virtual bool RankMemberIf(Func<string, double?, double, T, bool, bool> condition, string member, double score,
                                 T data = default(T))
        {
            return RankMemberIf(LeaderboardName, condition, member, score, data);
        }

        public virtual bool RankMemberIf(string leaderboardName, Func<string, double?, double, T, bool, bool> condition,
                                 string member, double score, T data = default(T))
        {
            double? currScore = GetScore(leaderboardName, member);

            if (condition(member, currScore, score, data, Reverse))
            {
                RankMember(leaderboardName, member, score, data);
                return true;
            }

            return false;
        }

        public virtual T GetMemberData(string member)
        {
            return GetMemberData(LeaderboardName, member);
        }

        public abstract T GetMemberData(string leaderboardName, string member);

        public virtual void UpdateMemberData(string member, T data)
        {
            UpdateMemberData(LeaderboardName, member, data);
        }

        public abstract void UpdateMemberData(string leaderboardName, string member, T data);

        public virtual void RemoveMemberData(string member)
        {
            RemoveMember(LeaderboardName, member);
        }

        public abstract void RemoveMemberData(string leaderboardName, string member);

        public virtual void RankMembers(IEnumerable<MemberScorePair> memberScores)
        {
            RankMembers(LeaderboardName, memberScores);
        }

        public abstract void RankMembers(string leaderboardName, IEnumerable<MemberScorePair> memberScores);

        public virtual void RemoveMember(string member)
        {
            RemoveMember(LeaderboardName, member);
        }

        public abstract void RemoveMember(string leaderboardName, string member);

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

        public virtual long TotalMembersInScoreRange(double minScore, double maxScore)
        {
            return TotalMembersInScoreRange(LeaderboardName, minScore, maxScore);
        }

        public abstract long TotalMembersInScoreRange(string leaderboardName, double minScore, double maxScore);

        public virtual double ChangeScore(string member, double scoreDelta)
        {
            return ChangeScore(LeaderboardName, member, scoreDelta);
        }

        public abstract double ChangeScore(string leaderboardName, string member, double scoreDelta);

        public virtual long? GetRank(string member)
        {
            return GetRank(LeaderboardName, member);
        }

        public abstract long? GetRank(string leaderboardName, string member);

        public virtual double? GetScore(string member)
        {
            return GetScore(LeaderboardName, member);
        }

        public abstract double? GetScore(string leaderboardName, string member);

        public virtual bool CheckMember(string member)
        {
            return CheckMember(LeaderboardName, member);
        }

        public virtual bool CheckMember(string leaderboardName, string member)
        {
            var score = GetScore(leaderboardName, member);
            return score.HasValue;
        }

        public virtual Record<T> GetRecord(string member)
        {
            return GetRecord(LeaderboardName, member);
        }

        public abstract Record<T> GetRecord(string leaderboardName, string member);

        public virtual void RemoveMembersInScoreRange(double minScore, double maxScore)
        {
            RemoveMembersInScoreRange(LeaderboardName, minScore, maxScore);
        }

        public abstract void RemoveMembersInScoreRange(string leaderboardName, double minScore, double maxScore);

        public virtual int? GetPercentile(string member)
        {
            return GetPercentile(LeaderboardName, member);
        }

        public abstract int? GetPercentile(string leaderboardName, string member);

        public virtual int GetPage(string member, int? pageSize = null)
        {
            return GetPage(LeaderboardName, member, pageSize);
        }

        public virtual int GetPage(string leaderboardName, string member, int? pageSize = null)
        {
            var rank = GetRank(leaderboardName, member);
            pageSize = ValidatePageSize(pageSize) ?? DEFAULT_PAGE_SIZE;

            if (rank == null)
            {
                rank = 0;
            }
            else
            {
                rank += 1;
            }

            return (int) Math.Ceiling((double) rank.Value / pageSize.Value);
        }

        public virtual IEnumerable<Record<T>> GetMembers(int page, LeaderboardOptions options = null)
        {
            return GetMembers(LeaderboardName, page, options);
        }

        public abstract IEnumerable<Record<T>> GetMembers(string leaderboardName, int page,
                                                          LeaderboardOptions options = null);

        public virtual IEnumerable<Record<T>> GetAllMembers(LeaderboardOptions options = null)
        {
            return GetAllMembers(LeaderboardName, options);
        }

        public abstract IEnumerable<Record<T>> GetAllMembers(string leaderboardName, LeaderboardOptions options = null);

        public virtual IEnumerable<Record<T>> GetMembersInScoreRange(double minScore, double maxScore, LeaderboardOptions options = null)
        {
            return GetMembersInScoreRange(LeaderboardName, minScore, maxScore, options);
        }

        public abstract IEnumerable<Record<T>> GetMembersInScoreRange(string leaderboardName, double minScore,
                                                                      double maxScore,
                                                                      LeaderboardOptions options = null);

        public virtual IEnumerable<Record<T>> GetMembersInRankRange(long startRank, long endRank, LeaderboardOptions options = null)
        {
            return GetMembersInRankRange(LeaderboardName, startRank, endRank, options);
        }

        public abstract IEnumerable<Record<T>> GetMembersInRankRange(string leaderboardName, long startRank, long endRank,
                                                                     LeaderboardOptions options = null);

        public virtual Record<T> GetMemberAt(long position, LeaderboardOptions options = null)
        {
            return GetMemberAt(LeaderboardName, position, options);
        }

        public virtual Record<T> GetMemberAt(string leaderboardName, long position, LeaderboardOptions options = null)
        {
            if (position <= TotalMembers(LeaderboardName))
            {
                options = options ?? new LeaderboardOptions();
                var pageSize = ValidatePageSize(options.PageSize) ?? PageSize;
                var currPage = (int) Math.Ceiling((double) position / pageSize);
                var offset = (int) ((position - 1) % pageSize);

                var members = GetMembers(leaderboardName, currPage, options);
                if (members != null && members.Any())
                {
                    var membersList = members.ToList();
                    return membersList[offset];
                }
            }

            return null;
        }

        public virtual IEnumerable<Record<T>> GetAroundMe(string member, LeaderboardOptions options = null)
        {
            return GetAroundMe(LeaderboardName, member, options);
        }

        public virtual IEnumerable<Record<T>> GetAroundMe(string leaderboardName, string member,
                                                          LeaderboardOptions options = null)
        {
            var reverseRank = GetRank(leaderboardName, member);
            if (reverseRank == null)
            {
                return new Record<T>[0];
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

        public virtual IEnumerable<Record<T>> GetRankedList(IEnumerable<string> members, LeaderboardOptions options = null)
        {
            return GetRankedList(LeaderboardName, members, options);
        }

        public abstract IEnumerable<Record<T>> GetRankedList(string leaderbaordName, IEnumerable<string> members,
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
