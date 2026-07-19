using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Primitives;
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

    JsonNode? IValueCipher.CreateEncryptedValue(JsonNode? plainNode, PlainKey key)
    {
        var nonce = nonceGenerator.NewNonce();
        var jsonStr = plainNode?.ToJsonString(serializerOptions) ?? JsonStringForNullValue;
        var bytes = jsonStr.ToUTF8Bytes();
        var encryptedData = symmetricCipher.Encrypt(key, new PlainData(bytes), nonce);
        var encryptedFieldValue = new EncryptedFieldValue(encryptedData, nonce, Version);
        var pack = encryptedFieldValue.Pack();
        return JsonValue.Create(pack);
    }

    JsonNode? IValueCipher.CreateDecryptedValue(JsonNode? encryptedNode, PlainKey key)
    {
        if (encryptedNode is not JsonValue jsonValue ||
            !jsonValue.TryGetValue<string>(out var pack) ||
            !EncryptedFieldValue.TryUnpack(pack, out var encryptedField))
        {
            return encryptedNode; // node is not an encrypted field, we return it as is
        }
        AssertCorrectVersion(encryptedField.Version);

        var bytes = symmetricCipher.Decrypt(key, encryptedField.EncryptedData, encryptedField.Nonce);
        var jsonStr = bytes.Bytes.ToUTF8String();
        return JsonNode.Parse(jsonStr);
    }

    // helper methods

    private static void AssertCorrectVersion(int version)
    {
        if (version != Version)
        {
            throw new InvalidOperationException($"Unsupported version: {version}");
        }
    }

}
