using Kintino.CipherConf.Models;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Models;

internal class RegexFilter : IFileFilter, IFieldFilter, ISerializable<RegexFilter>
{
    private readonly Regex? _regex;
    private readonly string? Pattern;

    public RegexFilter(string? regexPattern)
    {
        if (!string.IsNullOrEmpty(regexPattern))
        {
            _regex = new Regex(regexPattern);
            Pattern = regexPattern;
        }
    }
    public bool Match(string fieldName)
    {
        if (_regex == null)
        {
            return true; // If no regex is provided, match all fields.
        }
        return _regex.IsMatch(fieldName);
    }

    bool IFileFilter.Match(string fileFullPath) => this.Match(fileFullPath);

    bool IFieldFilter.Match(string fieldName) => this.Match(fieldName);

    public string Serialize()
    {
        return Pattern ?? string.Empty;
    }

    public static RegexFilter Deserialize(string? str)
    {
        return new RegexFilter(str);
    }
}
