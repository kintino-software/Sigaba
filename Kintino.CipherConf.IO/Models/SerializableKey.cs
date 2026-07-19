using Kintino.CipherConf.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.IO.Models;

internal class SerializableKey(EncryptedKey encryptedKey) : IJsonSerializable<SerializableKey>
{
    public EncryptedKey EncryptedKey => encryptedKey;

    // IJsonSerializable implementation

    public static SerializableKey DeserializeFromJsonString(string str)
    {
        var keyBase64 = JsonSerializer.Deserialize<string>(str)
            ?? throw new InvalidOperationException("Failed to deserialize the key.");
        var encryptedKey = new EncryptedKey(new EncryptedData(keyBase64.FromBase64String()));
        return new SerializableKey(encryptedKey);
    }

    public string SerializeToJsonString()
    {
        var keyBase64 = encryptedKey.Bytes.ToBase64String();
        return JsonSerializer.Serialize(keyBase64);
    }
}
