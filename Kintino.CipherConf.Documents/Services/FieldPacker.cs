using Kintino.CipherConf.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services;

internal class FieldPacker
{
    private record SerializationObj(string Nonce64, string EncryptedData64);

    public static string Pack(EncryptedData encryptedData, Nonce nonce)
    {
        // data to serialization object
        var nonce64 = nonce.Bytes.ToBase64String();
        var encryptedData64 = encryptedData.Bytes.ToBase64String();
        var serializationPath = new SerializationObj(nonce64, encryptedData64);

        // serialize to json and base64
        var json = JsonSerializer.Serialize(serializationPath)
            ?? throw new InvalidOperationException("Serialization failed");
        var bytes = json.ToUTF8Bytes();
        var json64 = bytes.ToBase64String();

        // wrap
        return Wrap(json64);
    }

    public static (EncryptedData, Nonce) Unpack(string package)
    {
        // unwrap
        var pack = Unwrap(package);

        // base64 -> json -> serialization object
        var bytes = pack.FromBase64String();
        var json = bytes.ToUTF8String();
        var serializationPack = JsonSerializer.Deserialize<SerializationObj>(json)
            ?? throw new InvalidOperationException("Deserialization failed");

        // serialization object -> EncryptedData and Nonce
        var nonceBytes = serializationPack.Nonce64.FromBase64String();
        var encryptedDataBytes = serializationPack.EncryptedData64.FromBase64String();
        return (new EncryptedData(encryptedDataBytes), new Nonce(new PlainData(nonceBytes)));
    }

    public static bool IsEncryptedFieldValue(string str)
    {
        return str.StartsWith("ENC(") && str.EndsWith(')');
    }

    private static string Wrap(string str)
    {
        return $"ENC({str})";
    }

    private static string Unwrap(string str)
    {
        if (!str.StartsWith("ENC(") || !str.EndsWith(')'))
        {
            throw new ArgumentException("Invalid wrapped encrypted value format", nameof(str));
        }
        var result = str[4..^1]; // Extract the wrapped part
        return result;
    }



}
