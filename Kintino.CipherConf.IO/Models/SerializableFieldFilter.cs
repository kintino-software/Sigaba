using Kintino.CipherConf.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Models;

internal class SerializableFieldFilter(string? includePattern) : IFieldFilter, IJsonSerializable<SerializableFieldFilter>
{
    private readonly Regex? regex = includePattern == null ? null : new Regex(includePattern);

    // IFieldFilter implementation

    bool IFieldFilter.Match(string fieldName)
    {
        return regex != null && regex.IsMatch(fieldName);
    }

    // ISerializable implementation

    public static SerializableFieldFilter DeserializeFromJsonString(string str)
    {
        var includePattern = JsonSerializer.Deserialize<string>(str);
        return new SerializableFieldFilter(includePattern);
    }
    public string SerializeToJsonString()
    {
        return JsonSerializer.Serialize(includePattern);
    }
}