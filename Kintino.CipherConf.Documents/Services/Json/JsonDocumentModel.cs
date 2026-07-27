using Kintino.CipherConf.Documents.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    public const string MetadataFieldName = "__metadata__";
    JsonByteScanner? byteScanner = null;
    private readonly List<JsonFieldReplacement> replacements = [];

    public DocumentMetadata Metadata { get; private set; } = new();

    // IDocumentModel implementation

    void IDocumentModel.Parse(string documentContent)
    {
        replacements.Clear();
        byteScanner = JsonByteScanner.Create(documentContent);
    }

    IEnumerable<string> IDocumentModel.GetFieldNames()
    {
        var scanner = GetScannerOrThrow();
        return scanner.KeyToFieldDataMap.Keys;
    }

    void IDocumentModel.SetFieldValue<T>(string key, [MaybeNull] T value)
    {
        if (value is null)
        {
            // Json values can be null or undefined, while C# values treat as the same thing.
            // So we must pass "null" string as a raw value to represent null values in the JSON document.
            replacements.Add(new(key, "null"));
            return;
        }
        var rawValue = JsonSerializer.Serialize(value);
        replacements.Add(new(key, rawValue));
    }

    void IDocumentModel.SetFieldRawValue(string key, string rawValue)
    {
        replacements.Add(new(key, rawValue));
    }

    string IDocumentModel.GetFieldRawValue(string key)
    {
        var scanner = GetScannerOrThrow();
        return scanner.GetRawValue(key)
            ?? throw new KeyNotFoundException($"Field '{key}' not found in the document.");
    }

    [return: MaybeNull]
    bool IDocumentModel.TryGetValue<T>(string key, [MaybeNull] out T value)
    {
        var scanner = GetScannerOrThrow();
        var rawValue = scanner.GetRawValue(key);

        if (rawValue is null)
        {
            // value not found, return false and set value to default
            value = default;
            return default;
        }

        try
        {
            // try to deserialize to the requested type. Deserialization may fail if the type is incompatible with the JSON value.
            value = JsonSerializer.Deserialize<T>(rawValue);
        }
        catch (JsonException)
        {
            value = default;
            return false;
        }
        return true;
    }

    string IDocumentModel.Serialize()
    {
        var scanner = GetScannerOrThrow();
        return scanner.Replace([.. replacements]);
    }

    // helpers

    private JsonByteScanner GetScannerOrThrow()
    {
        if (byteScanner is null)
            throw new InvalidOperationException("Document has not been parsed yet.");
        return byteScanner;
    }

    private static DocumentMetadata GetMetadata(JsonByteScanner scanner)
    {
        var metadataRaw = scanner.GetRawValue(MetadataFieldName);
        if (metadataRaw is null)
            return new DocumentMetadata();
        var metadata = JsonSerializer.Deserialize<DocumentMetadata>(metadataRaw);
        return metadata ?? new DocumentMetadata();
    }

    private static void SetMetadata(JsonByteScanner scanner, DocumentMetadata metadata)
    {
        var metadataRaw = JsonSerializer.Serialize(metadata);
        scanner.Replace(new JsonFieldReplacement(MetadataFieldName, metadataRaw));
    }
}
