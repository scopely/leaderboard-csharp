namespace Leaderboard
{
    public class MemberScorePair<K, V> where V: struct
    {
        public K Member { get; set; }
        public V Score { get; set; }

        public MemberScorePair(K member, V score)
        {
            Member = member;
            Score = score;
        }

        public MemberScorePair() : this(default(K), default(V))
        { }
    }
}
