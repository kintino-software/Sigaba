using Kintino.CipherConf.Documents.Models;

namespace Kintino.CipherConf.Documents.Services.Json;

internal record JsonDocumentNode(string Key, string Content) : IDocumentNode;
