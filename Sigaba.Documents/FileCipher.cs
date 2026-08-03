using Sigaba.Crypto;
using Sigaba.Documents.Models;
using Sigaba.Documents.Services;
using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.Documents;

internal class FileCipher(IFileSystem fs, ISymmetricCipher symmetricCipher, IAsymmetricCipher asymmetricCipher) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PublicKey publicKey, Predicate<string> fieldFilter)
    {
        var document = await LoadDocumentModelFromFileAsync(filePath);
        var fieldNames = document.GetFieldNames().Where(f => fieldFilter(f)).ToList();
        if (fieldNames.Count < 1)
            return;

        var plainKey = symmetricCipher.GenerateNewKey();
        var encryptedKey = asymmetricCipher.Encrypt(plainKey, publicKey);

        foreach (var fieldName in fieldNames)
        {
            // Should not re-encrypt fields that are already encrypted so we skip them
            if (document.TryGetValue<string>(fieldName, out var value) && FieldPacker.IsEncryptedFieldValue(value))
                continue;

            var rawValue = document.GetFieldRawValue(fieldName);
            var nonce = symmetricCipher.GenerateNewNonce();
            var encryptedData = symmetricCipher.Encrypt(plainKey, new PlainData(rawValue.ToUTF8Bytes()), nonce);

            var package = new EncryptedFieldPack(
                EncryptedKey: encryptedKey,
                EncryptedData: encryptedData,
                Nonce: nonce);

            var pack = FieldPacker.Pack(package);
            document.SetFieldValue(fieldName, pack);
        }

        await SaveChangedDocumentAsync(document, filePath);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PrivateKey privateKey)
    {
        var document = await LoadDocumentModelFromFileAsync(filePath);

        foreach (var field in document.GetFieldNames())
        {
            if (!document.TryGetValue<string>(field, out var value) ||  // field is not a string, it means that is not encrypted
                value is null ||                                        // field is null, also means is not encrypted
                !FieldPacker.IsEncryptedFieldValue(value))              // field is not encrypted
            {
                continue;
            }

            FieldPacker.Unpack(value).Deconstruct(out var encryptedKey, out var encryptedData, out var nonce);
            var plainKey = asymmetricCipher.Decrypt(encryptedKey, privateKey);

            var plainData = symmetricCipher.Decrypt(plainKey, encryptedData, nonce);
            var rawValue = plainData.Bytes.FromUtf8Bytes();
            document.SetFieldRawValue(field, rawValue);
        }

        await SaveChangedDocumentAsync(document, filePath);
    }

    // helpers

    private async Task<IDocumentModel> LoadDocumentModelFromFileAsync(string filePath)
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
