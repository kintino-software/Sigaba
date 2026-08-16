using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.Documents;

/// <summary>
/// Represents a service that can cipher and decipher fields in a document file using public and private keys.
/// </summary>
public interface IFileCipher
{
    /// <summary>
    /// Ciphers the specified file using the provided public key and field filter.
    /// </summary>
    /// <param name="filePath">The path of the file to cipher.</param>
    /// <param name="publicKey">The public key to use for ciphering.</param>
    /// <param name="fieldFilter">A predicate to filter which fields to cipher.</param>
    ValueTask CipherFile(FilePath filePath, PublicKey publicKey, Predicate<string> fieldFilter);

    /// <summary>
    /// Deciphers the specified file using the provided private key.
    /// </summary>
    /// <param name="filePath">The path of the file to decipher.</param>
    /// <param name="privateKey">The private key to use for deciphering.</param>
    ValueTask DecipherFile(FilePath filePath, PrivateKey privateKey);
}
