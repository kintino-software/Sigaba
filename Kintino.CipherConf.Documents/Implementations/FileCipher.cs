using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents.Implementations;

internal class FileCipher(
    IFileSystem fs,
    IRandomKeyGenerator randomKeyGenerator,
    INonceGenerator nonceGenerator,
    ISymmetricCipher symmetricCipher,
    IAsymmetricCipher asymmetricCipher) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(
        string filePath,
        PublicKey publicKey,
        IFieldFilter fieldFilter)
    {
        var document = await PrepareDocumentAsync(filePath);

        var fieldNames = document.GetFieldNames().Where(fieldFilter.Match).ToList();

        foreach (var field in fieldNames)
        {
            // Should not re-encrypt fields that are already encrypted so we skip them
            if (FieldPacker.IsEncryptedFieldValue(field))
                continue;
            var rawValue = document.GetFieldRawValue(field);
            var nonce = nonceGenerator.NewNonce();
            var plainKey = randomKeyGenerator.GenerateNewKey();
            var encryptedData = symmetricCipher.Encrypt(plainKey, new PlainData(rawValue.ToUTF8Bytes()), nonce);
            var encryptedKey = asymmetricCipher.Encrypt(plainKey, publicKey);

            var pack = FieldPacker.Pack(encryptedData, nonce, new EncryptedKey(encryptedKey));
            document.SetFieldValue(field, pack);
        }

        await SaveChangedDocumentAsync(document, filePath);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PrivateKey privateKey)
    {
        var document = await PrepareDocumentAsync(filePath);

        foreach (var field in document.GetFieldNames())
        {
            if (!document.TryGetValue<string>(field, out var value) ||  // field is not a string, it means that is not encrypted
                value is null ||                                        // field is null, also means is not encrypted
                !FieldPacker.IsEncryptedFieldValue(value))              // Skip fields that are not encrypted or null
            {
                continue;
            }

            var (encryptedData, nonce, encryptedKey) = FieldPacker.Unpack(value);
            var plainKey = asymmetricCipher.Decrypt(encryptedKey, privateKey);
            var plainData = symmetricCipher.Decrypt(new PlainKey(plainKey), encryptedData, nonce);
            var rawValue = plainData.Bytes.FromUtf8Bytes();
            document.SetFieldRawValue(field, rawValue);
        }

        await SaveChangedDocumentAsync(document, filePath);
    }

    // helpers

    private async Task<IDocumentModel> PrepareDocumentAsync(string filePath)
    {
        var document = DocumentModelFactory.GetDocumentModelByFileExtension(fs.Path.GetExtension(filePath));
        var content = await fs.File.ReadAllTextAsync(filePath);
        document.Parse(content);
        return document;
    }

    private async Task SaveChangedDocumentAsync(IDocumentModel document, string filePath)
    {
        var newContent = document.Serialize();
        await fs.File.WriteAllTextAsync(filePath, newContent);
    }
}
