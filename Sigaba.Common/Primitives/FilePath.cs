using Sigaba.Primitives.Base;
using System.IO.Abstractions;

namespace Sigaba.Primitives;

public class FilePath : BasePath
{
    public IFileSystem Fs { get; }
    public bool Exists { get => Fs.File.Exists(Path); }
    public string ExtensionWithDot { get => Fs.Path.GetExtension(Path) ?? throw new Exception($"Cannot get extension for file path '{Path}'."); }

    internal FilePath(IFileSystem fs, params string[] parts) : base(parts)
    {
        this.Fs = fs;
    }

    public DirPath Parent()
    {
        var dirPath = Fs.Path.GetDirectoryName(Path)
            ?? throw new InvalidOperationException($"Cannot get directory for file path '{Path}'.");
        return new DirPath(Fs, dirPath);
    }

    public Task WriteAsync(string content, bool overwrite, bool createFolders = true, CancellationToken cancellationToken = default)
    {
        if (!overwrite && Exists)
            throw new InvalidOperationException("File already exists and overwrite is set to false.");

        if (createFolders)
        {
            var dirPath = Fs.Path.GetDirectoryName(Path);
            if (!Fs.Directory.Exists(dirPath) && !string.IsNullOrWhiteSpace(dirPath))
                Fs.Directory.CreateDirectory(dirPath);
        }

        return Fs.File.WriteAllTextAsync(Path, content, cancellationToken);
    }

    public void Write(string content, bool overwrite, bool createFolders = true)
    {
        WriteAsync(content, overwrite, createFolders).GetAwaiter().GetResult();
    }

    public Task<string> ReadAsync(CancellationToken cancellationToken = default)
    {
        AssertExists();
        return Fs.File.ReadAllTextAsync(Path, cancellationToken);
    }

    public string Read()
    {
        return ReadAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public void AssertExists()
    {
        if (!Exists)
        {
            throw new FileNotFoundException($"File '{Path}' does not exist.");
        }
    }
}
