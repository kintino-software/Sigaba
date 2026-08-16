using Sigaba.Crypto;
using Sigaba.Documents.Models;
using Sigaba.Documents.Services;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;

namespace Sigaba.Documents;

internal partial class FileCipher(ICipher cipher)
{
    private static async Task<IDocumentModel> LoadDocumentModelFromFileAsync(FilePath filePath)
    {
        var document = DocumentModelFactory.GetDocumentModelByFilePath(filePath);
        var content = await filePath.ReadAsync();
        document.Parse(content);
        return document;
    }

    private static async Task SaveChangedDocumentAsync(IDocumentModel document, FilePath filePath)
    {
        var newContent = document.Serialize();
        await filePath.WriteAsync(newContent, overwrite: true);
    }

    public static bool IsEncryptedFieldValue(string? str)
    {
        return !string.IsNullOrEmpty(str) && str.StartsWith("ENC(") && str.EndsWith(')');
    }

    private static string Wrap(string str)
    {
        // must match the same format as Unwrap method
        return $"ENC({str})";
    }

    private static string Unwrap(string str)
    {
        // must match the same format as Wrap method
        if (!str.StartsWith("ENC(") || !str.EndsWith(')'))
        {
            throw new ArgumentException("Invalid wrapped encrypted value format", nameof(str));
        }
        var result = str[4..^1]; // Extract the wrapped part
        return result;
    }

    private static bool TryGetValueToEncrypt(IDocumentModel document, string fieldName, [NotNullWhen(true)] out string? rawValue)
    {
        rawValue = null;

        // First try to get the value as string to check if its encrypted or not
        // if the value is not even an string, means that is not already encripted.
        // We dont get the raw value at this point because each document would have it's own formarting
        // and we need an agnostic way to check if the value is already encrypted or not.
        if (document.TryGetValue<string>(fieldName, out var value))
        {
            // as the value is a string, we check if it is already encrypted, if so we skip it
            if (IsEncryptedFieldValue(value))
            {
                rawValue = null;
                return false;
            }
        }

        rawValue = document.GetFieldRawValue(fieldName);
        return true;
    }

    private static bool TryGetValueToDecrypt(IDocumentModel document, string fieldName, [NotNullWhen(true)] out string? value)
    {
        if (!document.TryGetValue<string>(fieldName, out value) || // field is not a string, it means that is not encrypted
            value is null ||                                        // field is null, also means is not encrypted
            !IsEncryptedFieldValue(value))                          // field is not encrypted
        {
            value = null;
            return false;
        }
        return true;
    }

    private string EncryptFieldValue(string rawValue, PublicKey publicKey)
    {
        var rawValueBytes = rawValue.ToUTF8Bytes(); // 1. raw -> bytes
        var plainData = new PlainData(rawValueBytes); // 2. bytes -> PlainData
        var encryptedData = cipher.EncryptWithKey(plainData, publicKey); // 3. PlainData -> EncryptedData
        var encryptedDataBase64 = encryptedData.ToBase64(); // 4. EncryptedData -> Base64 string
        var wraped = Wrap(encryptedDataBase64); // 5. Wrap

        return wraped;
    }

    private string DecryptFieldValue(string encryptedValue, PrivateKey privateKey)
    {
        var encryptedDataBase64 = Unwrap(encryptedValue); // 5. Unwrap
        var encryptedData = EncryptedData.FromBase64(encryptedDataBase64); // 4. EncryptedData <- Base64 string
        var plainData = cipher.DecryptWithKey(encryptedData, privateKey); // 3. PlainData <- EncryptedData
        var rawValueBytes = plainData.Bytes; // 2. bytes <- PlainData
        var rawValue = rawValueBytes.FromUtf8Bytes(); // 1. raw <- bytes
        return rawValue;
    }
}

internal partial class FileCipher : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(FilePath filePath, PublicKey publicKey, Predicate<string> fieldFilter)
    {
        var document = await LoadDocumentModelFromFileAsync(filePath);
        var fieldNames = document.GetFieldNames().Where(f => fieldFilter(f)).ToList();
        if (fieldNames.Count < 1)
            return;

        foreach (var fieldName in fieldNames)
        {
            if (TryGetValueToEncrypt(document, fieldName, out var rawValue))
            {
                var encripted = EncryptFieldValue(rawValue, publicKey);
                document.SetFieldValue(fieldName, encripted);
            }
        }

        await SaveChangedDocumentAsync(document, filePath);
    }

    async ValueTask IFileCipher.DecipherFile(FilePath filePath, PrivateKey privateKey)
    {
        var document = await LoadDocumentModelFromFileAsync(filePath);

        foreach (var field in document.GetFieldNames())
        {
            if (TryGetValueToDecrypt(document, field, out var value))
            {
                var rawValue = DecryptFieldValue(value, privateKey);
                document.SetFieldRawValue(field, rawValue);
            }
        }

        await SaveChangedDocumentAsync(document, filePath);
    }
}
