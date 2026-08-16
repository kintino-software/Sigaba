namespace Sigaba.Crypto.Services.Ciphers;

/// <summary>
/// Represents a cipher that has a version number associated with it.
/// It is intended to control which implementation of the cipher is used for any encryption or decryption operation, 
/// allowing for versioning and backward compatibility.
/// </summary>
internal interface IVersionedCipher : ICipher
{
    /// <summary>
    /// Gets the version number of the cipher.
    /// </summary>
    byte Version { get; }
}
