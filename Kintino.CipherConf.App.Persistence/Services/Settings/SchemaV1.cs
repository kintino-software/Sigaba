using System.Text.Json.Serialization;

namespace Kintino.CipherConf.App.Services.Settings;

public class SchemaV1 : ISettingsSchema
{
    [JsonPropertyName("version")]
    public int Version { get; } = 1;
    [JsonPropertyName("fieldRegex")]
    public required string FieldRegex { get; init; } = string.Empty;
    [JsonPropertyName("include")]
    public required string[] IncludeFileGlob { get; init; } = [];
    [JsonPropertyName("exclude")]
    public required string[] ExcludeFileGlob { get; init; } = [];
}
