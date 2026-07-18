using Kintino.CipherConf.IO.Models;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

internal class ConcreteContext : IContext
{
    internal record SerializationResult(string PrivateKeyStr, string PublicKeyStr, string SettingsStr);

    public required PrivateKey PrivateKey { get; init; }
    public required PublicKey PublicKey { get; init; }
    public required IFieldFilter FieldFilter { get; init; }
    public required IFileFilter FileFilter { get; init; }
    public required EncryptedKey Key { get; init; }

    public SerializationResult Serialize()
    {
        if (this.FieldFilter is not RegexFilter regexFieldFilter)
        {
            throw new InvalidOperationException($"FieldFilter must be of type {nameof(RegexFilter)}");
        }
        if (this.FileFilter is not RegexFilter regexFileFilter)
        {
            throw new InvalidOperationException($"FileFilter must be of type {nameof(RegexFilter)}");
        }

        var privateKeyStr = new SerializablePrivateKey(this.PrivateKey).Serialize();
        var publicKeyStr = new SerializablePublicKey(this.PublicKey).Serialize();
        var toolsSettingsStr = new ToolSettings()
        {
            PropertyRegex = regexFieldFilter.Serialize(),
            FileRegex = regexFileFilter.Serialize(),
            Key = this.Key.Bytes.ToBase64String(),
        }.Serialize();

        return new SerializationResult(privateKeyStr, publicKeyStr, toolsSettingsStr);
    }

    public static ConcreteContext Deserialize(string privateKeyStr, string publicKeyStr, string settingsStr)
    {
        var privateKey = SerializablePrivateKey.Deserialize(privateKeyStr).PrivateKey;
        var publicKey = SerializablePublicKey.Deserialize(publicKeyStr).PublicKey;
        var toolSettings = ToolSettings.Deserialize(settingsStr);
        var fieldFilter = RegexFilter.Deserialize(toolSettings.PropertyRegex);
        var fileFilter = RegexFilter.Deserialize(toolSettings.FileRegex);
        var key = new EncryptedKey(new EncryptedData(toolSettings.Key.FromBase64String()));

        return new ConcreteContext
        {
            PrivateKey = privateKey,
            PublicKey = publicKey,
            FieldFilter = fieldFilter,
            FileFilter = fileFilter,
            Key = key
        };
    }
}

