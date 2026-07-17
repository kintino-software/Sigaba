using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto;

/// <summary>
/// Represents an asymmetric cipher that can encrypt and decrypt data using public and private keys.
/// </summary>
public interface IAsymmetricCipher
{
    /// <summary>
    /// Creates a new pair of public and private keys for asymmetric encryption.
    /// </summary>
    /// <returns>A tuple containing the generated private key and public key.</returns>
    (PublicKey PublicKey, PrivateKey PrivateKey) CreateNewKeyPair();

    EncryptedData Encrypt(PlainData plainData, PublicKey publicKey);

    PlainData Decrypt(EncryptedData encryptedData, PrivateKey privateKey);
}
