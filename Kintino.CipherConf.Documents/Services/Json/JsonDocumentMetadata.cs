using Kintino.CipherConf.Documents.Models;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentMetadata
{
    public Dictionary<int, string> Keys { get; set; } = [];

    public DocumentMetadata ToDocumentMetadata() => new(this.Keys);

    public static JsonDocumentMetadata FromDocumentMetadata(DocumentMetadata metadata)
    {
        return new()
        {
            Keys = metadata.Base64EncryptedKeys.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }
}
