using System.IO.Abstractions.TestingHelpers;

namespace Sigaba;

public static class FileSystemExtensions
{
    extension(MockFileSystem fs)
    {
        public string RootDir { get => fs.Path.GetPathRoot(fs.AllPaths.First()); }

        public string SafePath(params string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = paths[i].Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            }
            return fs.Path.Combine(fs.RootDir, Path.Combine(paths));
        }

        public string SafeSetCwd(params string[] paths)
        {
            var cwd = fs.SafePath(paths);
            fs.AddDirectory(cwd);
            fs.Directory.SetCurrentDirectory(cwd);
            return cwd;
        }
    }
}
