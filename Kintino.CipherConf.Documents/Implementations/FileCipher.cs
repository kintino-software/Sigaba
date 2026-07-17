using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents.Implementations;

internal class FileCipher(IFileSystem fs, ICipherResolver documentCipherResolver) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher, IFieldFilter fieldFilter)
    {
        var documentCipher = documentCipherResolver.Resolve(filePath);
        var content = await fs.File.ReadAllTextAsync(filePath);
        var encryptedContent = documentCipher.Encrypt(plainKey, content, fieldFilter.Match);
        await fs.File.WriteAllTextAsync(filePath, encryptedContent);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher)
    {
        var documentCipher = documentCipherResolver.Resolve(filePath);
        var content = await fs.File.ReadAllTextAsync(filePath);
        var encryptedContent = documentCipher.Decrypt(plainKey, content);
        await fs.File.WriteAllTextAsync(filePath, encryptedContent);
    }
}
