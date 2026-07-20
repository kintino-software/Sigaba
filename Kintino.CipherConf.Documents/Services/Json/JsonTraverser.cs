using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

internal record JsonEntry(string? Key, int? Index, JsonNode Node)
{
    public override string ToString()
    {
        return $"Key: {Key}, Index: {Index}, Node: {Node}";
    }
}

internal static class JsonTraverser
{
    public static IEnumerable<JsonEntry> Traverse(JsonNode? node, string? key = null, int? index = null)
    {
        if (node == null)
        {
            yield break;
        }

        yield return new JsonEntry(key, index, node);


        switch (node)
        {
            case JsonObject jsonObject:
                foreach (var (k, child) in jsonObject)
                    foreach (var entry in Traverse(child, key: k))
                        yield return entry;
                break;
            case JsonArray jsonArray:
                foreach (var item in jsonArray)
                {
                    foreach (var entry in Traverse(item, index: jsonArray.IndexOf(item)))
                        yield return entry;
                }
                break;
        }
    }
}
