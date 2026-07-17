using Kintino.CipherConf.Models;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Primitives;

internal class RegexFileFilter : IFileFilter, ISerializable
{
    private readonly Regex? _regex;
    public RegexFileFilter(string? regexPattern)
    {
        if (!string.IsNullOrEmpty(regexPattern))
        {
            _regex = new Regex(regexPattern);
        }
    }
    public bool Match(string filePath)
    {
        if (_regex == null)
        {
            return true; // If no regex is provided, match all files.
        }
        return _regex.IsMatch(filePath);
    }
}