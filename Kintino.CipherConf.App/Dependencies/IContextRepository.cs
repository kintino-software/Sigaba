using Kintino.CipherConf.App.Models;

namespace Kintino.CipherConf.App.Dependencies;

/// <summary>
/// Represents a repository for managing contexts, allowing for the creation, retrieval, and existence checking of contexts based on a specified folder path.
/// </summary>
public interface IContextRepository
{
    /// <summary>
    /// Creates a new context with the specified initialization data and folder path.
    /// <br/>
    /// Implementations should throw an exception if a context already exists for the specified folder path.
    /// </summary>
    /// <param name="initData">The initialization data for the context.</param>
    /// <param name="folderPath">The folder path where the context will be stored.</param>
    /// <exception cref="InvalidOperationException">Thrown if a context already exists for the specified folder path.</exception>
    ValueTask CreateContext(InitData initData, string folderPath);
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
    ValueTask<Context> GetContext(string folderPath);
}
