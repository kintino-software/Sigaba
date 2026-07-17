using System.Text.RegularExpressions;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Models;

/// <summary>
/// Represents the context for the application.
/// Context is the information needed for the app to run.
/// </summary>
public record Context
{
    /// <summary>
    /// Gets or sets the private key associated with the context.
    /// </summary>
    public required PrivateKey PrivateKey { get; init; }
    /// <summary>
    /// Gets or sets the public key associated with the context.
    /// </summary>
    public required PublicKey? PublicKey { get; init; }
    /// <summary>
    /// Gets or sets the regular expression used to match properties.
    /// </summary>
    public required Regex? PropertyRegex { get; init; }
    /// <summary>
    /// Gets or sets the regular expression used to match files.
    /// </summary>
    public required Regex? FileRegex { get; init; }
    /// <summary>
    /// Gets or sets the cryptographic key associated with the context.
    /// </summary>
    public required CryptoKey Key { get; init; }
}
