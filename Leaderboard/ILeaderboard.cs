using System;
using System.Collections.Generic;

namespace Leaderboard
{
    public interface ILeaderboard<T>
    {
        string LeaderboardName { get; set; }
        int PageSize { get; set; }
        bool Reverse { get; set; }

        void DeleteLeaderboard();
        void DeleteLeaderboard(string leaderboardName);

        void RankMember(string member, double score, T data = default(T));
        void RankMember(string leaderboardName, string member, double score, T data = default(T));

        bool RankMemberIf(Func<string, double?, double, T, bool, bool> condition, string member, double score,
                          T data = default(T));

        bool RankMemberIf(string leaderboardName, Func<string, double?, double, T, bool, bool> condition, string member,
                          double score, T data = default(T));

        T GetMemberData(string member);
        T GetMemberData(string leaderboardName, string member);

        void UpdateMemberData(string member, T data);
        void UpdateMemberData(string leaderboardName, string member, T data);

        void RemoveMemberData(string member);
        void RemoveMemberData(string leaderboardName, string member);

        void RankMembers(IEnumerable<MemberScorePair> memberScores);
        void RankMembers(string leaderboardName, IEnumerable<MemberScorePair> memberScores);

        void RemoveMember(string member);
        void RemoveMember(string leaderboardName, string member);

        long TotalMembers();
        long TotalMembers(string leaderboardName);

        int TotalPages(int? pageSize = null);
        int TotalPages(string leaderboardName, int? pageSize = null);

        long TotalMembersInScoreRange(double minScore, double maxScore);
        long TotalMembersInScoreRange(string leaderboardName, double minScore, double maxScore);

        double ChangeScore(string member, double scoreDelta);
        double ChangeScore(string leaderboardName, string member, double scoreDelta);

        long? GetRank(string member);
        long? GetRank(string leaderboardName, string member);

        double? GetScore(string member);
        double? GetScore(string leaderboardName, string member);

        bool CheckMember(string member);
        bool CheckMember(string leaderboardName, string member);

        Record<T> GetRecord(string member);
        Record<T> GetRecord(string leaderboardName, string member);

        void RemoveMembersInScoreRange(double minScore, double maxScore);
        void RemoveMembersInScoreRange(string leaderboardName, double minScore, double maxScore);

        int? GetPercentile(string member);
        int? GetPercentile(string leaderboardName, string member);

        int GetPage(string member, int? pageSize = null);
        int GetPage(string leaderboardName, string member, int? pageSize = null);

        IEnumerable<Record<T>> GetMembers(int page, LeaderboardOptions options = null);
        IEnumerable<Record<T>> GetMembers(string leaderboardName, int page, LeaderboardOptions options = null);

        IEnumerable<Record<T>> GetAllMembers(LeaderboardOptions options = null);
        IEnumerable<Record<T>> GetAllMembers(string leaderboardName, LeaderboardOptions options = null);

        IEnumerable<Record<T>> GetMembersInScoreRange(double minScore, double maxScore, LeaderboardOptions options = null);
        IEnumerable<Record<T>> GetMembersInScoreRange(string leaderboardName, double minScore, double maxScore,
                                                   LeaderboardOptions options = null);

        IEnumerable<Record<T>> GetMembersInRankRange(long startRank, long endRank, LeaderboardOptions options = null);
        IEnumerable<Record<T>> GetMembersInRankRange(string leaderboardName, long startRank, long endRank,
                                                  LeaderboardOptions options = null);

        Record<T> GetMemberAt(long position, LeaderboardOptions options = null);
        Record<T> GetMemberAt(string leaderboardName, long position, LeaderboardOptions options = null);

        IEnumerable<Record<T>> GetAroundMe(string member, LeaderboardOptions options = null);
        IEnumerable<Record<T>> GetAroundMe(string leaderboardName, string member, LeaderboardOptions options = null);

        IEnumerable<Record<T>> GetRankedList(IEnumerable<string> members, LeaderboardOptions options = null);
        IEnumerable<Record<T>> GetRankedList(string leaderbaordName, IEnumerable<string> members,
                                             LeaderboardOptions options = null);
    }
}
