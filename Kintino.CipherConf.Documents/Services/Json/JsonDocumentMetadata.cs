using Kintino.CipherConf.Documents.Models;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentMetadata
{
    public Dictionary<int, string> Base64Keys { get; set; } = [];

    public DocumentMetadata ToDocumentMetadata() => new(this.Base64Keys);

    public static JsonDocumentMetadata FromDocumentMetadata(DocumentMetadata metadata)
    {
        return new()
        {
            Base64Keys = metadata.Base64Keys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }
}
