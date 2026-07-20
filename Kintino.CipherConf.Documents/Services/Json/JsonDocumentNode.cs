using Kintino.CipherConf.Documents.Models;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

internal record JsonDocumentNode : IDocumentNode
{
    public required JsonNode UnderlyingNode { get; init; }
    public required string Key { get; init; }
    public required string Content { get; init; }

}
