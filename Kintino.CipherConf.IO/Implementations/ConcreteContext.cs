using Kintino.CipherConf.IO.Models;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.IO.Implementations;

internal record SerializationResult(string PrivateKeyStr, string PublicKeyStr, string SettingsStr);

internal class ConcreteContext : IContext
{
    // IContext implementation

    public PrivateKey PrivateKey { get => SerializablePrivateKey.PrivateKey; }
    public PublicKey PublicKey { get => SerializablePublicKey.PublicKey; }
    public IFieldFilter FieldFilter { get => SerializableFieldFilter; }
    public IFileFilter FileFilter { get => SerializableFileFilter; }
    public EncryptedKey Key { get => SerializableKey.EncryptedKey; }

    // 

    public required SerializableFieldFilter SerializableFieldFilter { get; init; }
    public required SerializableFileFilter SerializableFileFilter { get; init; }
    public required SerializableKey SerializableKey { get; init; }
    public required SerializablePrivateKey SerializablePrivateKey { get; init; }
    public required SerializablePublicKey SerializablePublicKey { get; init; }


    public SerializationResult Serialize()
    {
        var root = new JsonObject()
        {
            ["fieldRegex"] = SerializableFieldFilter.SerializeToJsonString(),
            ["fileRegex"] = SerializableFileFilter.SerializeToJsonString(),
            ["key"] = SerializableKey.SerializeToJsonString()
        };

        return new SerializationResult(
            PrivateKeyStr: SerializablePrivateKey.SerializeToJsonString(),
            PublicKeyStr: SerializablePublicKey.SerializeToJsonString(),
            SettingsStr: root.ToJsonString(JsonConfig.SerializerOptions));
    }

    public static ConcreteContext Deserialize(string privateKeyStr, string publicKeyStr, string settingsStr)
    {
        var settings = JsonNode.Parse(settingsStr)
            ?? throw new ArgumentException("Settings string is not a valid JSON object.", nameof(settingsStr));
        var fieldFilter = SerializableFieldFilter.DeserializeFromJsonString(settings["fieldRegex"]?.ToString() ?? string.Empty);
        var fileFilter = SerializableFileFilter.DeserializeFromJsonString(settings["fileRegex"]?.ToString() ?? string.Empty);
        var key = SerializableKey.DeserializeFromJsonString(settings["key"]?.ToString() ?? string.Empty);

        return new ConcreteContext
        {
            SerializablePrivateKey = SerializablePrivateKey.DeserializeFromJsonString(privateKeyStr),
            SerializablePublicKey = SerializablePublicKey.DeserializeFromJsonString(publicKeyStr),
            SerializableFieldFilter = fieldFilter,
            SerializableFileFilter = fileFilter,
            SerializableKey = key
        };

    }
}

