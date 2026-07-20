using Kintino.CipherConf.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Models;

internal class CipherPack(EncryptedData encryptedData, Nonce nonce)
{
    private record SerializationObj(byte[] Nonce, byte[] EncryptedData);

    public EncryptedData EncryptedData { get; } = encryptedData;
    public Nonce Nonce { get; } = nonce;

    public string Pack()
    {
        var serializationPath = new SerializationObj(Nonce.Bytes, EncryptedData.Bytes);
        var serializationPackJson = JsonSerializer.SerializeToUtf8Bytes(serializationPath)
            ?? throw new InvalidOperationException("Serialization failed");
        var serializationPackBase64 = serializationPackJson.ToBase64String();
        return Wrap(serializationPackBase64);
    }

    private static CipherPack Unpack(string package)
    {
        var serializationPackBase64 = Unwrap(package);
        var serializationPackJson = serializationPackBase64.FromBase64String();
        var serializationPack = JsonSerializer.Deserialize<SerializationObj>(serializationPackJson)
            ?? throw new InvalidOperationException("Deserialization failed");
        return new CipherPack(
            new EncryptedData(serializationPack.EncryptedData),
            new Nonce(new PlainData(serializationPack.Nonce)));
    }

    public static bool TryUnpack(string? package, [NotNullWhen(true)] out CipherPack? result)
    {
        if (string.IsNullOrWhiteSpace(package) || !IsEncryptedFieldValue(package))
        {
            result = null;
            return false;
        }
        result = Unpack(package);
        return true;
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
