using Kintino.CipherConf.Documents.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    public const string DocumentMetadataKey = "__metadata__";
    private static readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        IndentSize = 4,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        AllowTrailingCommas = true,
        AllowDuplicateProperties = false,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = null
    };
    private readonly JsonSerializerOptions metaDataSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    private RawObject? rootMeta = null;
    public DocumentMetadata Metadata { get; private set; } = new([]);

    // serialization

    void IDocumentModel.Parse(string documentContent)
    {
        rootMeta = JsonSerializer.Deserialize<RawObject>(documentContent, serializerOptions)
            ?? throw new Exception("Could not parse document content");

        if (!rootMeta.TryGetChild<JsonDocumentMetadata>(DocumentMetadataKey, out var jsonMetadata, metaDataSerializerOptions))
        {
            jsonMetadata = new();
        }
        this.Metadata = jsonMetadata.ToDocumentMetadata();
    }

    string IDocumentModel.Serialize()
    {
        var root = GetRoot();
        root.SetChild(DocumentMetadataKey, JsonDocumentMetadata.FromDocumentMetadata(Metadata), metaDataSerializerOptions);
        return JsonSerializer.Serialize(root, serializerOptions);
    }

    // query

    IEnumerable<string> IDocumentModel.GetFieldNames() => GetRoot().GetFieldPaths();


    string IDocumentModel.GetFieldRawValue(string key)
    {
        var field = GetRoot().GetFieldByPath(key)
            ?? throw new KeyNotFoundException($"Key '{key}' not found in the document.");
        return field.RawValue;
    }

    bool IDocumentModel.TryGetValue<T>(string key, [MaybeNull] out T value)
    {
        try
        {
            var rawValue = ((IDocumentModel)this).GetFieldRawValue(key);
            value = rawValue == "null" ? default : JsonSerializer.Deserialize<T>(rawValue);
            return true;
        }
        catch (KeyNotFoundException)
        {
            value = default;
            return false;
        }
    }

    // modification

    void IDocumentModel.SetFieldValue<T>(string key, [MaybeNull] T value)
    {
        var rawValue = value is null ? "null" : JsonSerializer.Serialize(value);
        ((IDocumentModel)this).SetFieldRawValue(key, rawValue);
    }

    void IDocumentModel.SetFieldRawValue(string key, string rawValue)
    {
        var field = GetRoot().GetFieldByPath(key)
            ?? throw new KeyNotFoundException($"Key '{key}' not found in the document.");
        field.SetRawValue(rawValue);
    }

    // helpers

    private RawObject GetRoot([CallerMemberName] string? callerName = null)
    {
        if (rootMeta is null)
            throw new InvalidOperationException($"Root meta object is null. Ensure that the document has been parsed and metadata has been set. Caller: {callerName}");
        return rootMeta;
    }

}
