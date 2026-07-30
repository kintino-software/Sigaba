using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents;

internal class FileCipher(IFileSystem fs, ICipherFactory cipherFactory) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PublicKey publicKey, Predicate<string> fieldFilter)
    {
        var document = await LoadDocumentModelFromFileAsync(filePath);
        var symmetricCipher = cipherFactory.GetLatestSymmetricCipher();
        var asymmetricCipher = cipherFactory.GetLatestAsymmetricCipher();

        EncryptedKey? encryptedKey = null;
        PlainKey? plainKey = null;
        int keyIndex = -1;

        var fieldNames = document.GetFieldNames().Where(f => fieldFilter(f)).ToList();

        foreach (var field in fieldNames)
        {
            // Should not re-encrypt fields that are already encrypted so we skip them
            if (FieldPacker.IsEncryptedFieldValue(field))
                continue;

            // generate a single symmetric key per batch of fields
            if (encryptedKey == null || plainKey == null || keyIndex == -1)
            {
                plainKey = symmetricCipher.GenerateNewKey();
                encryptedKey = asymmetricCipher.Encrypt(plainKey, publicKey);
                document.Metadata.AddBase64Key(encryptedKey.ToBase64(), out keyIndex);
            }

            var rawValue = document.GetFieldRawValue(field);
            var nonce = symmetricCipher.GenerateNewNonce();
            var encryptedData = symmetricCipher.Encrypt(plainKey, new PlainData(rawValue.ToUTF8Bytes()), nonce);

            var package = new EncryptedFieldPack(
                KeyIndex: keyIndex,
                SymmetricCipherVersion: symmetricCipher.Version,
                AsymmetricCipherVersion: asymmetricCipher.Version,
                EncryptedData: encryptedData,
                Nonce: nonce);

            var pack = FieldPacker.Pack(package);
            document.SetFieldValue(field, pack);
        }

        await SaveChangedDocumentAsync(document, filePath);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PrivateKey privateKey)
    {
        var document = await LoadDocumentModelFromFileAsync(filePath);

        // cache decrypted keys for performance, so we don't have to decrypt the same key multiple times
        var plainKeys = new Dictionary<int, PlainKey>();

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

            var symmetricCipher = cipherFactory.GetSymmetricCipher(symmetricCipherVersion);
            var asymmetricCipher = cipherFactory.GetAsymmetricCipher(asymmetricCipherVersion);

            if (!plainKeys.TryGetValue(keyIndex, out var plainKey))
            {
                if (!document.Metadata.Base64EncryptedKeys.TryGetValue(keyIndex, out var encryptedBase64Key))
                    throw new Exception($"Could not find key for index {keyIndex} in document metadata");
                var encryptedKey = EncryptedKey.FromBase64(encryptedBase64Key);
                plainKey = asymmetricCipher.Decrypt(encryptedKey, privateKey);
                plainKeys[keyIndex] = plainKey;
            }

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
