using Kintino.CipherConf.Models;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Primitives;

internal class RegexFieldFilter : IFieldFilter, ISerializable
{
    private readonly Regex? _regex;
    public RegexFieldFilter(string? regexPattern)
    {
        if (!string.IsNullOrEmpty(regexPattern))
        {
            _regex = new Regex(regexPattern);
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
}
