using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Dependencies;

/// <summary>
/// Represents a generator for creating new nonces, 
/// which are unique values used in cryptographic operations to ensure security and prevent replay attacks.
/// </summary>
public interface INonceGenerator
{
    /// <summary>
    /// Generates a new random nonce.
    /// </summary>
    /// <returns>A new random <see cref="Nonce"/>.</returns>
    Nonce NewNonce();
}
