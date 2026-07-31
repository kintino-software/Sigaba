using Kintino.CipherConf.Documents.Models;
using System.Diagnostics.CodeAnalysis;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    private RawJsonSerializer serializer = null!;

    // serialization

    void IDocumentModel.Parse(string documentContent)
    {
        serializer = RawJsonSerializer.Create(documentContent);
    }

    string IDocumentModel.Serialize() => serializer.Serialize();

    // query

    IEnumerable<string> IDocumentModel.GetFieldNames() => serializer.KeyToPositionMap.Keys;

    string IDocumentModel.GetFieldRawValue(string key)
    {
        if (!serializer.TryGetRawValue(key, out var value))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in the document.");
        }
        return value;
    }

    bool IDocumentModel.TryGetValue<T>(string key, [MaybeNull] out T value) => serializer.TryGetValue(key, out value);

    // modification

    void IDocumentModel.SetFieldValue<T>(string key, [MaybeNull] T value) => serializer.Replace<T>(key, value);

    void IDocumentModel.SetFieldRawValue(string key, string rawValue) => serializer.Replace(key, rawValue);

}
