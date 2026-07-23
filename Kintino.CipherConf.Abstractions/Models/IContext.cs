using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Models;

/// <summary>
/// Represents the context for the application.
/// Context is the information needed for the app to run.
/// </summary>
public interface IContext
{
    /// <summary>
    /// Gets the private key associated with the context.
    /// </summary>
    PrivateKey? PrivateKey { get; }
    /// <summary>
    /// Gets the public key associated with the context.
    /// </summary>
    PublicKey? PublicKey { get; }
    /// <summary>
    /// Gets the predicate used to match properties.
    /// </summary>
    IFieldFilter FieldFilter { get; }
    /// <summary>
    /// Gets the predicate used to match files.
    /// </summary>
    IFileFilter FileFilter { get; }
}
