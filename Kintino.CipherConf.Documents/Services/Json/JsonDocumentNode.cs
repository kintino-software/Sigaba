using Kintino.CipherConf.Documents.Models;

namespace Kintino.CipherConf.Documents.Services.Json;

internal record JsonDocumentNode<TValue>(string Key, string RawContent, TValue Content) : IDocumentNode<TValue>;
