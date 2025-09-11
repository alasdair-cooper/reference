namespace AlasdairCooper.Reference.Shared.Common;

public sealed class MoneyJsonConverter : System.Text.Json.Serialization.JsonConverter<Money>
{
    public override Money Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options) => 
        Money.Parse(reader.GetString()!);

    public override void Write(System.Text.Json.Utf8JsonWriter writer, Money value, System.Text.Json.JsonSerializerOptions options) => 
        writer.WriteStringValue(value.ToString());
}