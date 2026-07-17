using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Dependencies;

/// <summary>
/// Represents a symmetric cipher that can encrypt and decrypt data using the same key for both operations.
/// </summary>
public interface ISymmetricCipher
{
    /// <summary>
    /// Encrypts the specified plain data using the given key and nonce.
    /// </summary>
    /// <param name="plainKey">The key to use for encryption.</param>
    /// <param name="plainData">The data to encrypt.</param>
    /// <param name="nonce">The nonce to use for encryption.</param>
    /// <returns>The encrypted data.</returns>
    CryptoBytes Encrypt(PlainKey plainKey, PlainBytes plainData, Nonce nonce);

    /// <summary>
    /// Decrypts the specified encrypted data using the given key and nonce.
    /// </summary>
    /// <param name="cipherKey">The key to use for decryption.</param>
    /// <param name="encryptedData">The data to decrypt.</param>
    /// <param name="nonce">The nonce to use for decryption.</param>
    /// <returns>The decrypted data.</returns>
    PlainBytes Decrypt(PlainKey cipherKey, CryptoBytes encryptedData, Nonce nonce);
}
