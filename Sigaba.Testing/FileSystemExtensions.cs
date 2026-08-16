using Sigaba.Primitives;
using Sigaba.Primitives.FileSystem;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba;

public static class FileSystemExtensions
{
  extension(MockFileSystem fs)
  {
    public string RootDir { get => fs.Path.GetPathRoot(fs.AllPaths.First()); }

    public string Combine(params string[] paths)
    {
      for (int i = 0; i < paths.Length; i++)
      {
        paths[i] = paths[i].Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
      }
      return fs.Path.Combine(fs.RootDir, Path.Combine(paths));
    }

    /// <summary>
    /// Sets the current working directory to the specified path(s). If the directory does not exist, it will be created. The method returns the full path of the new current working directory.
    /// </summary>
    /// <param name="segments">The path segments to combine into the new current working directory.</param>
    /// <returns>The full path of the new current working directory.</returns>
    public string SetCwd(params string[] segments)
    {
      var cwd = fs.Combine(segments);
      if (!fs.Directory.Exists(cwd))
        fs.Directory.CreateDirectory(cwd);
      if (fs.Directory.GetCurrentDirectory() != cwd)
        fs.Directory.SetCurrentDirectory(cwd);
      return cwd;
    }

    public FilePath AddFilePath2(string content, params string[] segments)
    {
      var filePath = fs.NewFilePath(segments);
      fs.AddFile(filePath.Path, new MockFileData(content ?? string.Empty));
      return filePath;
    }

    public DirPath AddDirPath(string path)
    {
      var dirPath = fs.NewDirPath(path);
      fs.AddDirectory(dirPath.Path);
      return dirPath;
    }
  }
}
