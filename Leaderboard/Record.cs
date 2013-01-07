namespace Leaderboard
{
    public class Record<K, V, T>
    {
        public K Member { get; protected set; }
        public V Score { get; protected set; }
        public long Rank { get; protected set; }
        public T Data { get; protected set; }

        public Record(K member, V score, long rank, T data = default(T))
        {
            Member = member;
            Score = score;
            Rank = rank;
            Data = data;
        }
    }
}
