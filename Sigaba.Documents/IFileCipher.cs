using Sigaba.Primitives;

namespace Sigaba.Documents;

public interface IFileCipher
{
    ValueTask CipherFile(string filePath, PublicKey publicKey, Predicate<string> fieldFilter);
    ValueTask DecipherFile(string filePath, PrivateKey privateKey);
}
