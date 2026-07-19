using Kintino.CipherConf.Models;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Implementations;

internal class FieldFilter(string? includePattern) : IFieldFilter
{
    public string? IncludePattern { get; } = includePattern;
    private readonly Regex? regex = includePattern == null ? null : new Regex(includePattern);

    bool IFieldFilter.Match(string fieldName)
    {
        return regex != null && regex.IsMatch(fieldName);
    }

}