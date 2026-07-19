using System.Text.Json;

namespace Kintino.CipherConf.IO.Models;

internal static class JsonConfig
{
    public readonly static JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower,
        WriteIndented = true,
        RespectNullableAnnotations = true,
    };
}
