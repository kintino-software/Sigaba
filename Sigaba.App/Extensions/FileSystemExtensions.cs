using System.IO.Abstractions;

namespace Sigaba.App.Extensions;

internal static class FileSystemExtensions
{
    extension(IFileSystem fs)
    {
        public void CreateFolderIfNotExists(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("Folder path cannot be null or whitespace.", nameof(folderPath));

            if (!fs.Directory.Exists(folderPath))
            {
                fs.Directory.CreateDirectory(folderPath);
            }
        }

        public string? GetNearestFileWithNameGoingUp(string startDirectory, string fileName)
        {
            if (string.IsNullOrWhiteSpace(startDirectory))
                throw new ArgumentException("Start directory cannot be null or whitespace.", nameof(startDirectory));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));

            var filePath = Path.Combine(startDirectory, fileName);
            while (!fs.File.Exists(filePath))
            {
                var curDir = fs.Path.GetDirectoryName(filePath);
                var parentDir = fs.Path.GetDirectoryName(curDir);
                if (string.IsNullOrEmpty(parentDir))
                    return null;
                filePath = Path.Combine(parentDir, fileName);
            }
            return filePath;
        }
    }
}
