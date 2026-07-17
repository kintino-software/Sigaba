using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Dependencies;

/// <summary>
/// Represents a generator for creating new random keys, 
/// which are used in cryptographic operations to ensure security and confidentiality.
/// </summary>
public interface IRandomKeyGenerator
{
    /// <summary>
    /// Generates a new random key.
    /// </summary>
    /// <returns>A new random <see cref="PlainKey"/>.</returns>
    PlainKey GenerateNewKey();
}
