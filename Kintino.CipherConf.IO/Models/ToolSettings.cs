using System.Text.Json;

namespace Kintino.CipherConf.IO.Models;

internal class ToolSettings : ISerializable<ToolSettings>
{
    public required string? PropertyRegex { get; init; }
    public required string? FileRegex { get; init; }
    public required string Key { get; init; }

    private readonly static JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        RespectNullableAnnotations = true
    };

    public static ToolSettings Deserialize(string str)
    {
        return JsonSerializer.Deserialize<ToolSettings>(str, jsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize ToolSettings.");
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this, jsonSerializerOptions);
    }
}
