using Sigaba.Primitives;

namespace Sigaba.Crypto;

public interface ICipher
{
    EncryptedData EncryptWithKey(IPlainData plainData, PublicKey publicKey);
    PlainData DecryptWithKey(IEncryptedData encryptedData, PrivateKey privateKey);
    EncryptedData EncryptWithPassword(IPlainData plainData, string password);
    PlainData DecryptWithPassword(IEncryptedData encryptedData, string password);
    (PublicKey, PrivateKey) GenerateKeys();
}
