using Kintino.CipherConf.IO;
using Kintino.CipherConf.Models;

namespace Kintino.CipherConf.IO;

/// <summary>
/// Represents an operation that can be performed on a temporary file, given its file path.
/// </summary>
/// <param name="tempFilePath">The path of the temporary file.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate ValueTask TempFileEditOperation(string tempFilePath);
/// <summary>
/// Represents an operation that can be performed before deleting a temporary file, given its file path.
/// </summary>
/// <param name="tempFilePath">The path of the temporary file.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate ValueTask TempFileBeforeDeleteOperation(string tempFilePath);

/// <summary>
/// Represents a file system abstraction that provides methods for interacting with the file system.
/// </summary>
public interface IFileOperations
{
    /// <summary>
    /// Gets the files from the specified directory that match the given search pattern.
    /// </summary>
    /// <param name="directory">The directory to search for files.</param>
    /// <param name="fileFilter">The filter to match file names against.</param>
    /// <returns>A collection of file paths that match the search pattern.</returns>
    public ValueTask<IEnumerable<string>> GetFilesFromDirectory(string directory, IFileFilter fileFilter);
    /// <summary>
    /// Creates a temporary file, performs the specified editing operation on it, and then executes the before-delete operation before deleting the temporary file.
    /// </summary>
    /// <param name="originalFile">The path of the original file to base the temporary file on.</param>
    /// <param name="editingOperation">The operation to perform on the temporary file.</param>
    /// <param name="beforeDeleteOperation">The operation to perform before deleting the temporary file.</param>
    public ValueTask WithTempFile(string originalFile, TempFileEditOperation editingOperation, TempFileBeforeDeleteOperation beforeDeleteOperation);
    /// <summary>
    /// Copies a file to a new location, overwriting the destination file if it already exists.
    /// </summary>
    /// <param name="originalFilePath">The path of the file to copy.</param>
    /// <param name="newFilePath">The path to copy the file to.</param>
    public ValueTask CopyWithOverwrite(string originalFilePath, string newFilePath);
}
