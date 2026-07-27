using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents.Implementations;

internal class FileCipher(
    IFileSystem fs,
    ICipherFactory cipherFactory) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PublicKey publicKey, IFieldFilter fieldFilter)
    {
        var document = await PrepareDocumentAsync(filePath);
        var symmetricCipher = cipherFactory.GetLatestSymmetricCipher();
        var asymmetricCipher = cipherFactory.GetLatestAsymmetricCipher();

        var key = symmetricCipher.GenerateNewKey();
        document.Metadata.AddBase64Key(key.ToBase64(), out var keyIndex);

        var fieldNames = document.GetFieldNames().Where(fieldFilter.Match).ToList();

        bool hasEncryptedFields = false;
        foreach (var field in fieldNames)
        {
            // Should not re-encrypt fields that are already encrypted so we skip them
            if (FieldPacker.IsEncryptedFieldValue(field))
                continue;

            hasEncryptedFields = true;
            var rawValue = document.GetFieldRawValue(field);
            var nonce = symmetricCipher.GenerateNewNonce();
            var plainKey = symmetricCipher.GenerateNewKey();
            var encryptedData = symmetricCipher.Encrypt(plainKey, new PlainData(rawValue.ToUTF8Bytes()), nonce);
            var encryptedKey = asymmetricCipher.Encrypt(plainKey, publicKey);

            var package = new EncryptedFieldPack(
                KeyIndex: keyIndex,
                SymmetricCipherVersion: symmetricCipher.Version,
                AsymmetricCipherVersion: asymmetricCipher.Version,
                EncryptedData: encryptedData,
                Nonce: nonce);

            var pack = FieldPacker.Pack(package);
            document.SetFieldValue(field, pack);
        }

        // if not field were encrypted, not need to persist any new key in the document metadata, so we remove it
        if (!hasEncryptedFields)
            document.Metadata.RemoveBase64Key(keyIndex);

        await SaveChangedDocumentAsync(document, filePath);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PrivateKey privateKey)
    {
        var document = await PrepareDocumentAsync(filePath);


        foreach (var field in document.GetFieldNames())
        {
            if (!document.TryGetValue<string>(field, out var value) ||  // field is not a string, it means that is not encrypted
                value is null ||                                        // field is null, also means is not encrypted
                !FieldPacker.IsEncryptedFieldValue(value))              // field is not encrypted
            {
                continue;
            }

            FieldPacker.Unpack(value).Deconstruct(
                out int keyIndex,
                out int symmetricCipherVersion,
                out int asymmetricCipherVersion,
                out EncryptedData encryptedData,
                out Nonce nonce);
            if (!document.Metadata.Base64Keys.TryGetValue(keyIndex, out var base64Key))
                continue; // TODO maybe throw?
            var encryptedKey = EncryptedKey.FromBase64(base64Key);

            var symmetricCipher = cipherFactory.GetSymmetricCipher(symmetricCipherVersion);
            var asymmetricCipher = cipherFactory.GetAsymmetricCipher(asymmetricCipherVersion);

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
