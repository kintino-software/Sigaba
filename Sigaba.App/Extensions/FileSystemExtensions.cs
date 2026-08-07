using System.IO.Abstractions;

namespace Sigaba.App.Extensions;

internal static class FileSystemExtensions
{
    extension(IFileSystem fs)
    {
        public void CreateFolderIfNotExists(string folderPath)
        {
            if (!fs.Directory.Exists(folderPath))
            {
                fs.Directory.CreateDirectory(folderPath);
            }
        }

        public string? GetNearestFileWithName(string startDirectory, string fileName)
        {
            var currentDirectory = startDirectory;
            while (!fs.File.Exists(fs.Path.Combine(currentDirectory, fileName)))
            {
                currentDirectory = fs.Path.GetDirectoryName(currentDirectory);
                if (string.IsNullOrEmpty(currentDirectory))
                    return null;
            }
            return currentDirectory;
        }
    }
}
