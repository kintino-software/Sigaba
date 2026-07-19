using Kintino.CipherConf.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Models;

internal class EncryptedFieldValue(EncryptedData encryptedData, Nonce nonce, int version)
{
    public EncryptedData EncryptedData { get; } = encryptedData;
    public Nonce Nonce { get; } = nonce;
    public int Version { get; } = version;

    private record SerializationObj(byte[] Nonce, byte[] Version, byte[] EncryptedData);

    public static bool TryUnpack(string? package, [NotNullWhen(true)] out EncryptedFieldValue? result)
    {
        if (string.IsNullOrWhiteSpace(package) || !IsWrapped(package))
        {
            result = null;
            return false;
        }
        result = Unpack(package);
        return true;
    }

    public string Pack()
    {
        var serializationPath = new SerializationObj(Nonce.Bytes, BitConverter.GetBytes(Version), EncryptedData.Bytes);
        var serializationPackJson = JsonSerializer.SerializeToUtf8Bytes(serializationPath)
            ?? throw new InvalidOperationException("Serialization failed");
        var serializationPackBase64 = serializationPackJson.ToBase64String();
        return Wrap(serializationPackBase64);
    }

    private static EncryptedFieldValue Unpack(string package)
    {

        var serializationPackBase64 = Unwrap(package);
        var serializationPackJson = serializationPackBase64.FromBase64String();
        var serializationPack = JsonSerializer.Deserialize<SerializationObj>(serializationPackJson)
            ?? throw new InvalidOperationException("Deserialization failed");
        return new EncryptedFieldValue(
            new EncryptedData(serializationPack.EncryptedData),
            new Nonce(new PlainData(serializationPack.Nonce)),
            BitConverter.ToInt32(serializationPack.Version));
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

    private static bool IsWrapped(string str)
    {
        return str.StartsWith("ENC(") && str.EndsWith(')');
    }


}
