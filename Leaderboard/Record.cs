namespace Leaderboard
{
    public class Record<K, V, T>
    {
        public K Member { get; set; }
        public V Score { get; set; }
        public long Rank { get; set; }
        public T Data { get; set; }

        public Record() {}

        public Record(K member, V score, long rank, T data = default(T))
        {
            Member = member;
            Score = score;
            Rank = rank;
            Data = data;
        }
    }
}
