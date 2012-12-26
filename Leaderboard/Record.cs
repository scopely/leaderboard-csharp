namespace Leaderboard
{
    public class Record<T>
    {
        public string Member { get; protected set; }
        public double Score { get; protected set; }
        public long Rank { get; protected set; }
        public T Data { get; protected set; }

        public Record(string member, double score, long rank, T data = default(T))
        {
            Member = member;
            Score = score;
            Rank = rank;
            Data = data;
        }
    }
}
