using BookSleeve;

namespace Leaderboard.Redis
{
    public static class RedisExtensions
    {
        public static RedisAggregate ToRedisAggregate(this Aggregate aggregate)
        {
            switch (aggregate)
            {
                case Aggregate.MAX:
                    return RedisAggregate.Max;
                case Aggregate.MIN:
                    return RedisAggregate.Min;
                default:
                    return RedisAggregate.Sum;
            }
        }
    }
}
