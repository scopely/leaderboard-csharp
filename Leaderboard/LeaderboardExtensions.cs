using BookSleeve;
using ServiceStack.Text;

namespace Leaderboard
{
    public static class LeaderboardExtensions
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

        public static string ToJson(this object obj)
        {
            return JsonSerializer.SerializeToString(obj);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.DeserializeFromString<T>(json);
        }
    }
}
