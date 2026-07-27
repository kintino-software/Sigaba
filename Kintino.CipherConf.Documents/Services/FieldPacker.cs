using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Primitives;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services;

internal class FieldPacker
{
    private record SerializationObj(int Scv, int Acv, int Kidx, string Nonce, string Data);

    public static string Pack(EncryptedFieldPack package)
    {
        package.Deconstruct(
            out var symmetricCipherVersion,
            out var asymmetricCipherVersion,
            out var keyIndex,
            out var encryptedData, out var nonce);

        // data to serialization object
        var serializationPath = new SerializationObj(
            Scv: symmetricCipherVersion,
            Acv: asymmetricCipherVersion,
            Kidx: keyIndex,
            Nonce: nonce.Bytes.ToBase64String(),
            Data: encryptedData.Bytes.ToBase64String());

        // serialize to json and base64
        var json = JsonSerializer.Serialize(serializationPath)
            ?? throw new InvalidOperationException("Serialization failed");
        var bytes = json.ToUTF8Bytes();
        var json64 = bytes.ToBase64String();

        // wrap
        return Wrap(json64);
    }

    public static EncryptedFieldPack Unpack(string package)
    {
        // unwrap
        var pack = Unwrap(package);

        // base64 -> json -> serialization object
        var bytes = pack.FromBase64String();
        var json = bytes.ToUTF8String();
        var serializationPack = JsonSerializer.Deserialize<SerializationObj>(json)
            ?? throw new InvalidOperationException("Deserialization failed");

        // serialization object -> result
        return new EncryptedFieldPack(
            SymmetricCipherVersion: serializationPack.Scv,
            AsymmetricCipherVersion: serializationPack.Acv,
            KeyIndex: serializationPack.Kidx,
            EncryptedData: new EncryptedData(serializationPack.Data.FromBase64String()),
            Nonce: new Nonce(new PlainData(serializationPack.Nonce.FromBase64String())))
        ;
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
