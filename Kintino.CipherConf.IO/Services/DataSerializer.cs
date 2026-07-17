using Kintino.CipherConf.IO.Primitives;
using Kintino.CipherConf.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.IO.Services;

internal class DataSerializer : IDataSerializer
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    string IDataSerializer.SerializeToolSettings(ToolSettings toolSettings)
    {
        var jsonStr = JsonSerializer.Serialize(toolSettings, _jsonSerializerOptions);
        return jsonStr;
    }

    ToolSettings IDataSerializer.DeserializeToolSettings(string jsonString)
    {
        var toolSettings = JsonSerializer.Deserialize<ToolSettings>(jsonString, _jsonSerializerOptions)
            ?? throw new InvalidOperationException("Deserialization failed.");
        return toolSettings;
    }

    PublicKey IDataSerializer.DeserializePublicKey(string str) => new(new PlainData(str.FromBase64String()));

    string IDataSerializer.SerializePublicKey(PublicKey publicKey) => publicKey.Bytes.ToBase64String();

    PrivateKey IDataSerializer.DeserializePrivateKey(string str) => new(new PlainData(str.FromBase64String()));

    string IDataSerializer.SerializePrivateKey(PrivateKey privateKey) => privateKey.Bytes.ToBase64String();
}