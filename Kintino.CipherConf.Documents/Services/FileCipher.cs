using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.Documents.Services;

internal class FileCipher(IFileSystem fs, ICipherResolver documentCipherResolver) : IFileCipher
{
    async ValueTask IFileCipher.CipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher, Regex? propertyRegex)
    {
        var documentCipher = documentCipherResolver.Resolve(filePath);
        var content = await fs.File.ReadAllTextAsync(filePath);
        var encryptedContent = documentCipher.Encrypt(
            plainKey,
            content,
            propertyRegex == null ? AllTruePredicate : propertyRegex.IsMatch);
        await fs.File.WriteAllTextAsync(filePath, encryptedContent);
    }

    async ValueTask IFileCipher.DecipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher)
    {
        var documentCipher = documentCipherResolver.Resolve(filePath);
        var content = await fs.File.ReadAllTextAsync(filePath);
        var encryptedContent = documentCipher.Decrypt(plainKey, content);
        await fs.File.WriteAllTextAsync(filePath, encryptedContent);
    }

    private static bool AllTruePredicate(string propertyName) => true;

}
