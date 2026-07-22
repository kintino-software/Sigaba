using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents.Implementations;

internal class FileCipher(IFileSystem fs, INonceGenerator nonceGenerator) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher, IFieldFilter fieldFilter)
    {
        var content = await fs.File.ReadAllTextAsync(filePath);
        var documentCipher = new DocumentCipher(symmetricCipher, nonceGenerator);
        var newContent = documentCipher.Encrypt(DocumentType.Json, content, plainKey, fieldFilter.Match);
        await fs.File.WriteAllTextAsync(filePath, newContent);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher)
    {
        var content = await fs.File.ReadAllTextAsync(filePath);
        var documentCipher = new DocumentCipher(symmetricCipher, nonceGenerator);
        var newContent = documentCipher.Decrypt(DocumentType.Json, content, plainKey);
        await fs.File.WriteAllTextAsync(filePath, newContent);
    }


}
