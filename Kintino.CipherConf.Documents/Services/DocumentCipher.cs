using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services.Json;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Services;

internal class DocumentCipher(ISymmetricCipher symmetricCipher, INonceGenerator nonceGenerator)
{
    public string Encrypt(
        DocumentType documentType,
        string content,
        PlainKey plainKey,
        Predicate<string> propertyNameFilter)
    {
        var document = CreateDocumentModel(documentType);
        return document.Transform(content, node =>
        {
            if (CipherPack.IsEncryptedFieldValue(node.Content))
                return node.Content;
            var nonce = nonceGenerator.NewNonce();
            var encryptedBytes = symmetricCipher.Encrypt(plainKey, new PlainData(node.Content.ToUTF8Bytes()), nonce);
            var encryptedFieldValue = new CipherPack(encryptedBytes, nonce);
            var pack = encryptedFieldValue.Pack();
            return pack;
        });
    }

    public string Decrypt(DocumentType documentType, string content, PlainKey plainKey)
    {
        var document = CreateDocumentModel(documentType);
        return document.Transform(content, node =>
        {
            if (!CipherPack.TryUnpack(node.Content, out var encryptedField))
                return node.Content;
            var plainData = symmetricCipher.Decrypt(plainKey, encryptedField.EncryptedData, encryptedField.Nonce);
            var content = plainData.Bytes.ToUTF8String();
            return content;
        });
    }

    private IDocumentModel CreateDocumentModel(DocumentType documentType)
    {
        return documentType switch
        {
            DocumentType.Json => new JsonDocumentModel(),
            _ => throw new InvalidOperationException($"Document type '{documentType}' is not supported."),
        };
    }
}
