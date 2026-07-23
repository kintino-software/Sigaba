using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents;

/// <summary>
/// Represents a file cipher that can encrypt and decrypt files using a specified symmetric cipher and key.
/// The cipher should only encrypt property values, not property names, and should be able to handle files with multiple properties.
/// </summary>
public interface IFileCipher
{
    /// <summary>
    /// Encrypts the specified file using the provided symmetric cipher and key, and optionally filters properties to encrypt based on a regex pattern.
    /// </summary>
    /// <param name="filePath">The path of the file to encrypt.</param>
    /// <param name="publicKey">The public key to use for encryption.</param>
    /// <param name="fieldFilter">A filter to select which properties to encrypt.</param>
    ValueTask CipherFile(string filePath, PublicKey publicKey, IFieldFilter fieldFilter);
    /// <summary>
    /// Decrypts the specified file using the provided symmetric cipher and key. The implementation should know which properties were encrypted and handle them accordingly.
    /// </summary>
    /// <remarks>
    /// For implementations: NO property filtering with regex here as the implementation should know which property was encrypted and which was not. 
    /// The implementation should handle the decryption of the encrypted properties and leave the unencrypted properties intact.
    /// </remarks>
    /// <param name="filePath">The path of the file to decrypt.</param>
    /// <param name="privateKey">The private key to use for decryption.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask DecipherFile(string filePath, PrivateKey privateKey);
}
