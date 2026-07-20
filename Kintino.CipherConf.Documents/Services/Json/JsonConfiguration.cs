using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

internal static class JsonConfiguration
{
    public static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        WriteIndented = true,
        RespectNullableAnnotations = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
    };
}
