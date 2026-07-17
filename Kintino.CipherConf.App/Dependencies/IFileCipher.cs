using System.Text.RegularExpressions;
using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Dependencies;

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
    /// <param name="plainKey">The plain key to use for encryption.</param>
    /// <param name="symmetricCipher">The symmetric cipher to use for encryption.</param>
    /// <param name="propertyRegex">An optional regex pattern to filter properties to encrypt.</param>
    ValueTask CipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher, Regex? propertyRegex);
    /// <summary>
    /// Decrypts the specified file using the provided symmetric cipher and key. The implementation should know which properties were encrypted and handle them accordingly.
    /// </summary>
    /// <remarks>
    /// For implementations: NO property filtering with regex here as the implementation should know which property was encrypted and which was not. 
    /// The implementation should handle the decryption of the encrypted properties and leave the unencrypted properties intact.
    /// </remarks>
    /// <param name="filePath">The path of the file to decrypt.</param>
    /// <param name="plainKey">The plain key to use for decryption.</param>
    /// <param name="symmetricCipher">The symmetric cipher to use for decryption.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask DecipherFile(string filePath, PlainKey plainKey, ISymmetricCipher symmetricCipher);
}
