using Sigaba.Primitives;

namespace Sigaba.Crypto;

public interface ICipher
{
    PlainData Decrypt(EncryptedData encryptedData, PrivateKey privateKey);
    EncryptedData Encrypt(PlainData plainData, PublicKey publicKey);
    (PublicKey, PrivateKey) GenerateKeys();
}
