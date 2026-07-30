using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents;

public interface IFileCipher
{
    ValueTask CipherFile(string filePath, PublicKey publicKey, Predicate<string> fieldFilter);
    ValueTask DecipherFile(string filePath, PrivateKey privateKey);
}
