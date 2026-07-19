using Kintino.CipherConf.Models;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Implementations;


internal class FileFilter(string? includePattern, string? excludePattern) : IFileFilter
{
    public string? IncludePattern { get; } = includePattern;
    public string? ExcludePattern { get; } = excludePattern;

    private readonly Regex? includeRegex = includePattern == null ? null : new Regex(includePattern);
    private readonly Regex? excludeRegex = excludePattern == null ? null : new Regex(excludePattern);

    bool IFileFilter.Match(string fileFullPath)
    {
        return (includeRegex == null || includeRegex.IsMatch(fileFullPath)) &&
               (excludeRegex == null || !excludeRegex.IsMatch(fileFullPath));
    }
}
