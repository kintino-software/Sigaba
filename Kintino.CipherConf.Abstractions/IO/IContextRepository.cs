using Kintino.CipherConf.Models;

namespace Kintino.CipherConf.IO;

/// <summary>
/// Represents a repository for managing contexts, allowing for the creation, retrieval, and existence checking of contexts based on a specified folder path.
/// </summary>
public interface IContextRepository
{
    /// <summary>
    /// Saves a new context with the specified initialization data and folder path.
    /// <br/>
    /// Implementations should throw an exception if a context already exists for the specified folder path.
    /// </summary>
    /// <param name="context">The context to be saved.</param>
    /// <param name="folderPath">The folder path where the context will be stored.</param>
    /// <exception cref="InvalidOperationException">Thrown if a context already exists for the specified folder path.</exception>
    ValueTask SaveContext(IContext context, string folderPath);
    /// <summary>
    /// Checks if a context exists for the specified folder path.
    /// </summary>
    /// <param name="folderPath">The folder path to check for an existing context.</param>
    /// <returns>True if a context exists for the specified folder path; otherwise, false.</returns>
    ValueTask<bool> HasContext(string folderPath);
    /// <summary>
    /// Retrieves the context for the specified folder path.
    /// </summary>
    /// <param name="folderPath">The folder path to retrieve the context for.</param>
    /// <returns>The context for the specified folder path.</returns>
    ValueTask<IContext> GetContext(string folderPath);
}
