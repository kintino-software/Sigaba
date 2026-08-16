using Sigaba.Crypto;
using Sigaba.Documents.Models;
using Sigaba.Documents.Services;
using Sigaba.Primitives;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.Documents;

internal class FileCipher(ICipher cipher) : IFileCipher
{
  async ValueTask IFileCipher.CipherFile(FilePath filePath, PublicKey publicKey, Predicate<string> fieldFilter)
  {
    var document = await LoadDocumentModelFromFileAsync(filePath);
    var fieldNames = document.GetFieldNames().Where(f => fieldFilter(f)).ToList();
    if (fieldNames.Count < 1)
      return;

    foreach (var fieldName in fieldNames)
    {
      // Should not re-encrypt fields that are already encrypted so we skip them
      if (document.TryGetValue<string>(fieldName, out var value) && IsEncryptedFieldValue(value))
        continue;

      var rawValue = document.GetFieldRawValue(fieldName);
      var rawValueBytes = rawValue.ToUTF8Bytes();
      var plainData = new PlainData(rawValueBytes);

      var encryptedData = cipher.EncryptWithKey(plainData, publicKey);

      var encryptedDataBase64 = encryptedData.ToBase64();
      var wraped = Wrap(encryptedDataBase64);
      document.SetFieldValue(fieldName, wraped);
    }

    await SaveChangedDocumentAsync(document, filePath);
  }

  async ValueTask IFileCipher.DecipherFile(FilePath filePath, PrivateKey privateKey)
  {
    var document = await LoadDocumentModelFromFileAsync(filePath);

    foreach (var field in document.GetFieldNames())
    {
      if (!document.TryGetValue<string>(field, out var value) ||  // field is not a string, it means that is not encrypted
          value is null ||                                        // field is null, also means is not encrypted
          !IsEncryptedFieldValue(value))                          // field is not encrypted
      {
        continue;
      }

      var encryptedData64 = Unwrap(value);
      var encryptedData = EncryptedData.FromBase64(encryptedData64);

      var plainData = cipher.DecryptWithKey(encryptedData, privateKey);

      var rawValue = plainData.Bytes.FromUtf8Bytes();
      document.SetFieldRawValue(field, rawValue);
    }

    await SaveChangedDocumentAsync(document, filePath);
  }

  // helpers

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
}
