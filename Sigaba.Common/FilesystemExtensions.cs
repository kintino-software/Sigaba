using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba;

public static class FilesystemExtensions
{
    extension(IFileSystem fs)
    {
        public FilePath CreateFile(params string[] parts) => new(fs, parts);

        public DirPath CreateDir(params string[] parts) => new(fs, parts);
        public DirPath CreateCwdDir() => new(fs, fs.Directory.GetCurrentDirectory());
    }
}