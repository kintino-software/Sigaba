using Sigaba.Primitives.FileSystem;
using Sigaba.Primitives.FileSystem.Base;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Sigaba.Primitives;

public class DirPath(IFileSystem fs, params string[] parts) : BasePath(fs, parts)
{
    public bool Exists { get => Fs.Directory.Exists(Path); }

    public DirPath? Parent()
    {
        var parentDir = Fs.Path.GetDirectoryName(Path);
        if (string.IsNullOrWhiteSpace(parentDir))
            return null;
        return new DirPath(Fs, parentDir);
    }

    public DirPath CombineAsDir(params string[] parts)
    {
        var combinedPath = Fs.Path.Combine([Path, .. parts]);
        return new DirPath(Fs, combinedPath);
    }

    public FilePath CombineAsFile(params string[] parts)
    {
        var combinedPath = Fs.Path.Combine([Path, .. parts]);
        return new FilePath(Fs, combinedPath);
    }

    public void EnsureCreated()
    {
        if (!Exists)
        {
            Fs.Directory.CreateDirectory(Path);
        }
    }

    public bool TryGetNearestFileWithNameGoingUp(string fileName, [NotNullWhen(true)] out FilePath? foundFilePath)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));

        for (var curDir = this; curDir != null; curDir = curDir.Parent())
        {
            var filePath = curDir.CombineAsFile(fileName);
            if (filePath.Exists)
            {
                foundFilePath = filePath;
                return true;
            }
        }

        foundFilePath = null;
        return false;
    }
}
