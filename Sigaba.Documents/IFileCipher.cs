using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.Documents;

public interface IFileCipher
{
    ValueTask CipherFile(FilePath filePath, PublicKey publicKey, Predicate<string> fieldFilter);
    ValueTask DecipherFile(FilePath filePath, PrivateKey privateKey);
}
