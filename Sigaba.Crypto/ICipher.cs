using Sigaba.Primitives;

namespace Sigaba.Crypto;

public interface ICipher
{
  EncryptedData EncryptWithKey(PlainData plainData, PublicKey publicKey);
  PlainData DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey);
  EncryptedData EncryptWithPassword(PlainData plainData, string password);
  PlainData DecryptWithPassword(EncryptedData encryptedData, string password);
  (PublicKey, PrivateKey) GenerateKeys();
}
