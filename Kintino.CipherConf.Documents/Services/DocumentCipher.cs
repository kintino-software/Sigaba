using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.Services;

internal class DocumentCipher(ISymmetricCipher symmetricCipher, INonceGenerator nonceGenerator)
{
    public void Encrypt(IDocumentModel document, PlainKey plainKey, Predicate<string> propertyNameFilter)
    {
        var nodes = document.GetNodes().Where(n => propertyNameFilter(n.Key));
        foreach (var node in nodes)
        {
            if (CipherPack.IsEncryptedFieldValue(node.Content))
                continue;
            var nonce = nonceGenerator.NewNonce();
            var encryptedBytes = symmetricCipher.Encrypt(plainKey, new PlainData(node.Content.ToUTF8Bytes()), nonce);
            var encryptedFieldValue = new CipherPack(encryptedBytes, nonce);
            var pack = encryptedFieldValue.Pack();
            document.UpdateNodeContent(node, pack);
        }
    }

    public void Decrypt(IDocumentModel document, PlainKey plainKey)
    {
        var nodes = document.GetNodes();
        foreach (var node in nodes)
        {
            if (!CipherPack.TryUnpack(node.Content, out var encryptedField))
                continue;
            var plainData = symmetricCipher.Decrypt(plainKey, encryptedField.EncryptedData, encryptedField.Nonce);
            var content = plainData.Bytes.ToUTF8String();
            document.UpdateNodeContent(node, content);
        }
    }
}
