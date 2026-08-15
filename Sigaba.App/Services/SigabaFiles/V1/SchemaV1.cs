using System.Text.Json.Serialization;

namespace Sigaba.App.Services.SigabaFiles.V1;

public record SchemaV1
{
    public record ConfigurationSchema
    {
        [JsonPropertyName("fieldRegex")]
        public required string FieldRegex { get; init; } = string.Empty;
        [JsonPropertyName("include")]
        public required string[] IncludeFileGlob { get; init; } = [];
        [JsonPropertyName("exclude")]
        public required string[] ExcludeFileGlob { get; init; } = [];
    }

    [JsonPropertyName("version")]
    public int Version { get; } = 1;
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; init; }
    [JsonPropertyName("publicKey")]
    public required string PublicKeyBase64 { get; init; }

    [JsonPropertyName("configuration")]
    public required ConfigurationSchema Configuration { get; init; }


}
