using System;
using System.Collections.Generic;

namespace Leaderboard
{
    public interface ILeaderboard<K, V, T> where V: struct
    {
        string LeaderboardName { get; set; }
        int PageSize { get; set; }
        bool Reverse { get; set; }

        void DeleteLeaderboard();
        void DeleteLeaderboard(string leaderboardName);

        void RankMember(K member, V score, T data = default(T));
        void RankMember(string leaderboardName, K member, V score, T data = default(T));

        bool RankMemberIf(Func<K, V?, V, T, bool, bool> condition, K member, V score,
                          T data = default(T));

        bool RankMemberIf(string leaderboardName, Func<K, V?, V, T, bool, bool> condition, K member,
                          V score, T data = default(T));

        T GetMemberData(K member);
        T GetMemberData(string leaderboardName, K member);

        void UpdateMemberData(K member, T data);
        void UpdateMemberData(string leaderboardName, K member, T data);

        void RemoveMemberData(K member);
        void RemoveMemberData(string leaderboardName, K member);

        void RankMembers(IEnumerable<MemberScorePair<K, V>> memberScores);
        void RankMembers(string leaderboardName, IEnumerable<MemberScorePair<K,V>> memberScores);

        void RemoveMember(K member);
        void RemoveMember(string leaderboardName, K member);

        long TotalMembers();
        long TotalMembers(string leaderboardName);

        int TotalPages(int? pageSize = null);
        int TotalPages(string leaderboardName, int? pageSize = null);

        long TotalMembersInScoreRange(V minScore, V maxScore);
        long TotalMembersInScoreRange(string leaderboardName, V minScore, V maxScore);

        V ChangeScore(K member, V scoreDelta);
        V ChangeScore(string leaderboardName, K member, V scoreDelta);

        long? GetRank(K member);
        long? GetRank(string leaderboardName, K member);

        V? GetScore(K member);
        V? GetScore(string leaderboardName, K member);

        bool CheckMember(K member);
        bool CheckMember(string leaderboardName, K member);

        Record<K, V, T> GetRecord(K member);
        Record<K, V, T> GetRecord(string leaderboardName, K member);

        void RemoveMembersInScoreRange(V minScore, V maxScore);
        void RemoveMembersInScoreRange(string leaderboardName, V minScore, V maxScore);

        int? GetPercentile(K member);
        int? GetPercentile(string leaderboardName, K member);

        int GetPage(K member, int? pageSize = null);
        int GetPage(string leaderboardName, K member, int? pageSize = null);

        IEnumerable<Record<K, V, T>> GetMembers(int page, LeaderboardOptions options = null);
        IEnumerable<Record<K,V, T>> GetMembers(string leaderboardName, int page, LeaderboardOptions options = null);

        IEnumerable<Record<K, V, T>> GetAllMembers(LeaderboardOptions options = null);
        IEnumerable<Record<K, V, T>> GetAllMembers(string leaderboardName, LeaderboardOptions options = null);

        IEnumerable<Record<K, V, T>> GetMembersInScoreRange(V minScore, V maxScore, LeaderboardOptions options = null);
        IEnumerable<Record<K, V, T>> GetMembersInScoreRange(string leaderboardName, V minScore, V maxScore,
                                                            LeaderboardOptions options = null);

        IEnumerable<Record<K, V, T>> GetMembersInRankRange(long startRank, long endRank, LeaderboardOptions options = null);
        IEnumerable<Record<K, V, T>> GetMembersInRankRange(string leaderboardName, long startRank, long endRank,
                                                           LeaderboardOptions options = null);

        Record<K, V, T> GetMemberAt(long position, LeaderboardOptions options = null);
        Record<K, V, T> GetMemberAt(string leaderboardName, long position, LeaderboardOptions options = null);

        IEnumerable<Record<K, V, T>> GetAroundMe(K member, LeaderboardOptions options = null);
        IEnumerable<Record<K, V, T>> GetAroundMe(string leaderboardName, K member, LeaderboardOptions options = null);

        IEnumerable<Record<K, V, T>> GetRankedList(IEnumerable<K> members, LeaderboardOptions options = null);
        IEnumerable<Record<K, V, T>> GetRankedList(string leaderbaordName, IEnumerable<K> members,
                                                   LeaderboardOptions options = null);
    }
}
