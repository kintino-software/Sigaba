using Kintino.CipherConf.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Models;


internal class SerializableFileFilter(string? includePattern, string? excludePattern) : IFileFilter, IJsonSerializable<SerializableFileFilter>
{
    private record InitWrapper(string? IncludeRegex, string? ExcludeRegex);

    private readonly Regex? includeRegex = includePattern == null ? null : new Regex(includePattern);
    private readonly Regex? excludeRegex = excludePattern == null ? null : new Regex(excludePattern);

    // IFileFilter implementation

    bool IFileFilter.Match(string fileFullPath)
    {
        return (includeRegex == null || includeRegex.IsMatch(fileFullPath)) &&
               (excludeRegex == null || !excludeRegex.IsMatch(fileFullPath));
    }

    // ISerializable implementation

    public static SerializableFileFilter DeserializeFromJsonString(string str)
    {
        var config = JsonSerializer.Deserialize<InitWrapper>(str, JsonConfig.SerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize FileFilterConfiguration.");
        return new SerializableFileFilter(config.IncludeRegex, config.ExcludeRegex);
    }

    public string SerializeToJsonString()
    {
        return JsonSerializer.Serialize(new InitWrapper(includePattern, excludePattern), JsonConfig.SerializerOptions);
    }
}
