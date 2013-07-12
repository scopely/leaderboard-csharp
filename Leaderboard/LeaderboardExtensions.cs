using ServiceStack.Text;

namespace Leaderboard
{
    public static class LeaderboardExtensions
    {
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
