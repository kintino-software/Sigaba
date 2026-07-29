using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kintino.CipherConf.Documents.Services.Json.Converters;

internal class RawConverter : JsonConverter<RawObject>
{
    public override RawObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }
        return ReadMetaObject(ref reader);
    }

    private static RawObject ReadMetaObject(ref Utf8JsonReader reader)
    {
        var fields = new List<RawField>();
        var children = new Dictionary<string, RawObject>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new RawObject(fields, children);
            }
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString() ?? throw new JsonException("Could not get property name.");
                reader.Read();
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var child = ReadMetaObject(ref reader);
                    children.Add(propertyName, child);
                }
                else
                {
                    var value = GetRawJsonValue(ref reader);
                    fields.Add(new RawField(propertyName, value));
                }
            }
        }
        throw new JsonException("Unexpected end of JSON");
    }

    public static string GetRawJsonValue(ref Utf8JsonReader reader)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        return jsonDocument.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, RawObject value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var field in value.Fields)
        {
            writer.WritePropertyName(field.Key);
            using var jsonDocument = JsonDocument.Parse(field.RawValue);
            jsonDocument.RootElement.WriteTo(writer);
        }
        foreach (var child in value.Children)
        {
            writer.WritePropertyName(child.Key);
            Write(writer, child.Value, options);
        }
        writer.WriteEndObject();
    }
}
