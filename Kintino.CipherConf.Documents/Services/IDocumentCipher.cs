
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.Documents.Services;

public interface IDocumentCipher
{
    string Decrypt(PlainKey key, string documentContent);
    string Encrypt(PlainKey key, string documentContent, Predicate<string>? propertyNameFilter);
}
