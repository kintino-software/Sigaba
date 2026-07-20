using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Documents.Services.Json;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents.Implementations;

internal class FileCipher(IFileSystem fs, DocumentCipher documentCipher) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher, IFieldFilter fieldFilter)
    {
        var content = await fs.File.ReadAllTextAsync(filePath);
        var document = CreateDocumentModel(filePath);
        document.Deserialize(content);
        documentCipher.Encrypt(document, plainKey, fieldFilter.Match);
        var encryptedContent = document.Serialize();
        await fs.File.WriteAllTextAsync(filePath, encryptedContent);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher)
    {
        var content = await fs.File.ReadAllTextAsync(filePath);
        var document = CreateDocumentModel(filePath);
        document.Deserialize(content);
        documentCipher.Decrypt(document, plainKey);
        var decryptedContent = document.Serialize();
        await fs.File.WriteAllTextAsync(filePath, decryptedContent);
    }

    private IDocumentModel CreateDocumentModel(string filePath)
    {
        var extension = fs.Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".json" => new JsonDocumentModel(),
            _ => throw new InvalidOperationException($"File extension '{extension}' is not supported."),
        };
    }
}
