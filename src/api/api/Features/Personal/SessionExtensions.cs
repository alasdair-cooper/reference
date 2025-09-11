using System.Text.Json;
using AlasdairCooper.Reference.Api.Utilities;

namespace AlasdairCooper.Reference.Api.Features.Personal;

public static class SessionExtensions
{
    extension(ISession session)
    {
        public BasketStore Basket => new(session);

        public T? Get<T>(string key) =>
            session.GetString(key) is { } val && JsonSerializer.Deserialize<T>(val, JsonSerializerOptions.Session) is { } res ? res : default;

        public void Set<T>(string key, T value) => session.SetString(key, JsonSerializer.Serialize(value, JsonSerializerOptions.Session));
    }
}