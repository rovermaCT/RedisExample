using System.Runtime.CompilerServices;
using System.Text.Json;

namespace RedisCacheExample.Extensions
{
    public static class SessionExtensions
    {
        // Serialize and save any object type to Redis
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            session.SetString(key, JsonSerializer.Serialize(value, jsonOptions));
        }

        // Retrieve and deserialize any object type from Redis
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null) return default;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<T>(value, jsonOptions);
        }
    }
}
