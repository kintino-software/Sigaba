using System.Diagnostics.CodeAnalysis;

namespace Kintino.CipherConf.Documents.Models;

internal interface IDocumentModel
{
    DocumentMetadata Metadata { get; }
    void Parse(string documentContent);
    IEnumerable<string> GetFieldNames();
    void SetFieldValue<T>(string key, [MaybeNull] T value);
    void SetFieldRawValue(string key, string rawValue);
    string GetFieldRawValue(string key);
    bool TryGetValue<T>(string key, [MaybeNull] out T value);
    string Serialize();
}
