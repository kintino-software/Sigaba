using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.Documents.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class ValueCipher(ISymmetricCipher symmetricCipher, INonceGenerator nonceGenerator) : IValueCipher
{
    public const int Version = 1;
    public const string JsonStringForNullValue = "null";
    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        RespectNullableAnnotations = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default,
    };

    JsonNode? IValueCipher.CreateEncryptedValue(JsonNode? originalPlainJsonValue, PlainKey key)
    {
        var nonce = nonceGenerator.NewNonce();
        var type = originalPlainJsonValue?.GetValueKind() ?? JsonValueKind.Null;
        var plainJsonString = originalPlainJsonValue?.ToJsonString(serializerOptions) ?? JsonStringForNullValue; // jsonNodes with null values are null, so we need to handle that case by serializing string "null" explicitly
        var cipherBytes = Encrypt(plainJsonString, key, nonce);
        var pack = new CipherPack(cipherBytes, nonce, type, Version).Pack();
        return JsonValue.Create(EncWrap(pack));
    }

    JsonNode? IValueCipher.CreateDecryptedValue(JsonNode? originalEncryptedJsonValue, PlainKey key)
    {
        if (originalEncryptedJsonValue == null)
            return null;
        var jsonPack = originalEncryptedJsonValue.GetValue<string>();
        var pack = CipherPack.Unpack(EncUnwrap(jsonPack));
        AssertCorrectVersion(pack.Version);
        var jsonNode = Decrypt(pack.CipherBytes, key, pack.Nonce);
        AssertCorrectType(pack.ValueKind, jsonNode);
        return jsonNode;
    }

    bool IValueCipher.IsEncrypted(JsonNode? jsonValue)
    {
        if (jsonValue == null || jsonValue.GetValueKind() != JsonValueKind.String)
        {
            return false;
        }

        var str = jsonValue.GetValue<string>();
        return str.StartsWith("ENC(") && str.EndsWith(')');
    }



    // helper methods

    private CryptoBytes Encrypt(JsonNode? jsonNode, PlainKey key, Nonce nonce)
    {
        var plainJsonString = jsonNode?.ToJsonString(serializerOptions) ?? JsonStringForNullValue;
        var plainBytes = new PlainBytes(plainJsonString.ToUTF8Bytes());
        return symmetricCipher.Encrypt(key, plainBytes, nonce);
    }

    private JsonNode? Decrypt(CryptoBytes cipherBytes, PlainKey key, Nonce nonce)
    {
        var plainBytes = symmetricCipher.Decrypt(key, cipherBytes, nonce);
        var jsonStr = plainBytes.Bytes.Value.ToUTF8String();
        var plainNode = JsonNode.Parse(JsonSerializer.Deserialize<string>(jsonStr)!);
        return plainNode;
    }

    private static void AssertCorrectVersion(int version)
    {
        if (version != Version)
        {
            throw new InvalidOperationException($"Unsupported version: {version}");
        }
    }

    private static void AssertCorrectType(JsonValueKind type, JsonNode? jsonNode)
    {
        var actualType = jsonNode?.GetValueKind() ?? JsonValueKind.Null;
        if (actualType != type)
        {
            throw new InvalidOperationException($"Type mismatch: expected {type}, but got {actualType}");
        }
    }

    private static string EncWrap(string str)
    {
        return $"ENC({str})";
    }

    private static string EncUnwrap(string str)
    {
        if (!str.StartsWith("ENC(") || !str.EndsWith(')'))
        {
            throw new ArgumentException("Invalid wrapped encrypted value format", nameof(str));
        }
        var result = str[4..^1]; // Extract the wrapped part
        return result;
    }

}
