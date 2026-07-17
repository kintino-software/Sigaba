using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.IO.Primitives;
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

    PublicKey IDataSerializer.DeserializePublicKey(string str)
    {
        var string64 = new String64(str);
        var bytes = string64.AsBytes();
        var publicKey = new PublicKey(bytes);
        return publicKey;
    }

    string IDataSerializer.SerializePublicKey(PublicKey publicKey)
    {
        var string64 = publicKey.Bytes.AsBase64();
        return string64.Value;
    }

    PrivateKey IDataSerializer.DeserializePrivateKey(string str)
    {
        var string64 = new String64(str);
        var bytes = string64.AsBytes();
        var privateKey = new PrivateKey(bytes);
        return privateKey;
    }

    string IDataSerializer.SerializePrivateKey(PrivateKey privateKey)
    {
        var string64 = privateKey.Bytes.AsBase64();
        return string64.Value;
    }


}
