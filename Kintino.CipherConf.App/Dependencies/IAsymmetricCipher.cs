using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Dependencies;

/// <summary>
/// Represents an asymmetric cipher that can encrypt and decrypt data using public and private keys.
/// </summary>
public interface IAsymmetricCipher
{
    /// <summary>
    /// Creates a new pair of public and private keys for asymmetric encryption.
    /// </summary>
    /// <param name="publicKey">The generated public key.</param>
    /// <param name="privateKey">The generated private key.</param>
    void CreateNewKeyPair(out PublicKey publicKey, out PrivateKey privateKey);
    /// <summary>
    /// Encrypts the given plain bytes using the specified public key.
    /// </summary>
    /// <param name="plainBytes">The plain bytes to encrypt.</param>
    /// <param name="publicKey">The public key to use for encryption.</param>
    /// <returns>The encrypted bytes.</returns>
    CryptoBytes Encrypt(PlainBytes plainBytes, PublicKey publicKey);
    /// <summary>
    /// Decrypts the given encrypted bytes using the specified private key.
    /// </summary>
    /// <param name="cryptoBytes">The encrypted bytes to decrypt.</param>
    /// <param name="privateKey">The private key to use for decryption.</param>
    /// <returns>The decrypted bytes.</returns>
    PlainBytes Decrypt(CryptoBytes cryptoBytes, PrivateKey privateKey);
}
