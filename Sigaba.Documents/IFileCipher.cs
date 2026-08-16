using Sigaba.Primitives;

namespace Sigaba.Documents;

public interface IFileCipher
{
  ValueTask CipherFile(FilePath filePath, PublicKey publicKey, Predicate<string> fieldFilter);
  ValueTask DecipherFile(FilePath filePath, PrivateKey privateKey);
}
