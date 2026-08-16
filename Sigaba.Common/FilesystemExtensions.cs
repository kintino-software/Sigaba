using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba;

public static class FilesystemExtensions
{
  extension(IFileSystem fs)
  {
    public FilePath NewFilePath(params string[] parts) => new(fs, parts);

    public DirPath NewDirPath(params string[] parts) => new(fs, parts);

    public DirPath NewCwdDirPath() => new(fs, fs.Directory.GetCurrentDirectory());
  }

}