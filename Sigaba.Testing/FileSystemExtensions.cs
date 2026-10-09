using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba;

[ExcludeFromCodeCoverage]
public static class FileSystemExtensions
{
    extension(MockFileSystem fs)
    {
        /// <summary>
        /// Adds a <see cref="FilePath"/> to the mock file system with the specified content and segments.
        /// </summary>
        /// <param name="content">The content of the mock file.</param>
        /// <param name="segments">The path segments for the mock file.</param>
        /// <returns>The created <see cref="FilePath"/>.</returns>
        public FilePath AddMockFilePath(string content, FilePath filePath)
        {
            fs.AddFile(filePath, new MockFileData(content ?? string.Empty));
            return filePath;
        }

        /// <summary>
        /// Adds a <see cref="DirPath"/> to the mock file system with the specified path.
        /// </summary>
        /// <param name="segments">The path segments of the mock directory.</param>
        /// <returns>The created <see cref="DirPath"/>.</returns>
        public DirPath AddMockDirPath(DirPath dirPath)
        {
            fs.AddDirectory(dirPath);
            return dirPath;
        }
    }
}
