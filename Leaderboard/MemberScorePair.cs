namespace Leaderboard
{
    public class MemberScorePair
    {
        public string Member { get; set; }
        public double Score { get; set; }

        public MemberScorePair(string member, double score)
        {
            Member = member;
            Score = score;
        }

        public MemberScorePair() : this(null, 0.0)
        { }
    }
}
