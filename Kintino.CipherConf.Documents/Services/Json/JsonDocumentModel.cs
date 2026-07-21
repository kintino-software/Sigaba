using Kintino.CipherConf.Documents.Models;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    internal JsonNode? RootNode { get; private set; }

    public void Deserialize(string content)
    {
        try
        {
            RootNode = JsonNode.Parse(content) ?? throw new InvalidOperationException("Invalid JSON document");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to deserialize JSON document", ex);
        }
    }

    public string Serialize()
    {
        return RootNode?.ToJsonString(JsonConfiguration.SerializerOptions)
            ?? throw new InvalidOperationException("No json document loaded.");
    }

    public IEnumerable<IDocumentNode> GetNodes()
    {
        return JsonTraverser.Traverse(RootNode)
            .Where(entry => entry.Key != null)
            .Select(entry => new JsonDocumentNode
            {
                Key = entry.Key!,
                Content = entry.Node.ToJsonString(JsonConfiguration.SerializerOptions),
                UnderlyingNode = entry.Node
            });
    }

    public void UpdateNodeContent(IDocumentNode node, string newContent)
    {
        if (node is not JsonDocumentNode jsonNode)
            throw new InvalidOperationException("Invalid node type");
        var newNode = JsonNode.Parse(newContent)
            ?? throw new InvalidOperationException("Invalid JSON content");
        jsonNode.UnderlyingNode.ReplaceWith(newNode);
    }
}
