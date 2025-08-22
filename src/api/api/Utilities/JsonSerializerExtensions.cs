using System.Text.Json;
using System.Text.Json.Serialization;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Utilities;

public static class JsonSerializerOptionsExtensions
{
    extension(JsonSerializerOptions)
    {
        public static JsonSerializerOptions Session
        {
            get
            {
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                options.Converters.AddDefaultConverters();
                return options;
            }
        }
    }

    extension(ICollection<JsonConverter> converters)
    {
        public void AddDefaultConverters()
        {
            converters.Add(new MoneyJsonConverter());
            converters.Add(new JsonStringEnumConverter());
        }
    }
}