using System.IO.Abstractions;

namespace Kintino.CipherConf.IO.Services;

internal class FileCrawler(IFileSystem fs) : IFileCrawler
{
     IEnumerable<string> IFileCrawler.Crawl(string rootDirFullPath, bool scanDeep)
    {
        if (string.IsNullOrEmpty(rootDirFullPath))
        {
            throw new ArgumentException("Root directory path cannot be null or empty.", nameof(rootDirFullPath));
        }
        if (!System.IO.Directory.Exists(rootDirFullPath))
        {
            throw new System.IO.DirectoryNotFoundException($"The directory '{rootDirFullPath}' does not exist.");
        }
        var searchOption = scanDeep ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
        return fs.Directory.EnumerateFiles(rootDirFullPath, "*", searchOption);
    }
}
