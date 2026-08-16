using Sigaba.Primitives.Crypto;

namespace Sigaba.Crypto;

/// <summary>
/// Represents a service that can encrypt and decrypt data using public/private keys or passwords.
/// </summary>
public interface ICipher
{
    /// <summary>
    /// Encrypts the given plain data using the provided public key.
    /// </summary>
    /// <param name="plainData">The plain data to encrypt.</param>
    /// <param name="publicKey">The public key to use for encryption.</param>
    /// <returns>The encrypted data.</returns>
    EncryptedData EncryptWithKey(PlainData plainData, PublicKey publicKey);

    /// <summary>
    /// Decrypts the given encrypted data using the provided private key.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="privateKey">The private key to use for decryption.</param>
    /// <returns>The decrypted plain data.</returns>
    PlainData DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey);

    /// <summary>
    /// Encrypts the given plain data using the provided password.
    /// </summary>
    /// <param name="plainData">The plain data to encrypt.</param>
    /// <param name="password">The password to use for encryption.</param>
    /// <returns>The encrypted data.</returns>
    EncryptedData EncryptWithPassword(PlainData plainData, string password);

    /// <summary>
    /// Decrypts the given encrypted data using the provided password.
    /// </summary>
    /// <param name="encryptedData">The encrypted data to decrypt.</param>
    /// <param name="password">The password to use for decryption.</param>
    /// <returns>The decrypted plain data.</returns>
    PlainData DecryptWithPassword(EncryptedData encryptedData, string password);

    /// <summary>
    /// Generates a new pair of public and private keys.
    /// </summary>
    /// <returns>A tuple containing the generated public and private keys.</returns>
    (PublicKey, PrivateKey) GenerateKeys();
}
