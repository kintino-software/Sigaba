using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

internal record JsonEntry(string? Key, int? Index, JsonNode Node)
{
    public override string ToString()
    {
        return $"Key: {Key}, Index: {Index}, Node: {Node}";
    }
}
