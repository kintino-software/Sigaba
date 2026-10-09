using Sigaba.Primitives.FileSystem;
using System.IO.Abstractions;

namespace Sigaba;

public static class FileSystemExtensions
{
    extension(IFileSystem fs)
    {
        /// <summary>
        /// Ensures that the specified directory exists. If the directory does not exist, it is created.
        /// </summary>
        public void EnsureDirectoryExists(DirPath dirPath)
        {
            if (!fs.Directory.Exists(dirPath))
            {
                fs.Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// Writes the specified content to a file asynchronously, ensuring the containing directory exists and optionally preventing overwriting.
        /// </summary>
        public Task SafeWriteAllTextAsync(FilePath filePath, string content, bool allowOverwrite)
        {
            fs.EnsureDirectoryExists(filePath.GetContainingDirectory());
            if (!allowOverwrite && fs.File.Exists(filePath))
            {
                throw new InvalidOperationException($"File '{filePath}' already exists and overwriting is not allowed.");
            }
            return fs.File.WriteAllTextAsync(filePath, content);
        }

    }
}
