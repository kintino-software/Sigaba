using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Models;

/// <summary>
/// Represents the minimum data required to create a context.
/// </summary>
public record InitData
{
    /// <summary>
    /// Gets or sets the folder path associated with the context.
    /// </summary>
    public required string FolderPath { get; init; }
    /// <summary>
    /// Gets or sets the private key associated with the context.
    /// </summary>
    public required PrivateKey PrivateKey { get; init; }
    /// <summary>
    /// Gets or sets the public key associated with the context.
    /// </summary>
    public required PublicKey PublicKey { get; init; }
    /// <summary>
    /// Gets or sets the regular expression used to match properties.
    /// </summary>
    public required string? PropertyRegex { get; init; }
    /// <summary>
    /// Gets or sets the regular expression used to match files.
    /// </summary>
    public required string? FileRegex { get; init; }
    /// <summary>
    /// Gets or sets the cryptographic key associated with the context.
    /// </summary>
    public required CryptoKey Key { get; init; }
}
