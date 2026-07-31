using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto;

/// <summary>
/// Represents an asymmetric cipher that can encrypt and decrypt data using public and private keys.
/// </summary>
public interface IAsymmetricCipher
{
    (PublicKey PublicKey, PrivateKey PrivateKey) CreateNewKeyPair();

    EncryptedKey Encrypt(PlainKey plainData, PublicKey publicKey);

    PlainKey Decrypt(EncryptedKey encryptedData, PrivateKey privateKey);
}
