using Kintino.CipherConf.Documents.Models;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    private record Replacement(int Start, int OriginalLength, byte[] NewValueBytes);

    public static DocumentType DocumentType { get; } = DocumentType.Json;

    public string Transform<TNewValue>(string documentContent, Func<FieldKey, FieldRawValue, TNewValue> transform, Func<FieldKey, bool> predicate)
    {
        var scanner = JsonByteScanner.Create(documentContent);
        return scanner.Replace((field) =>
        {
            var newValue = transform(field.Key, field.RawValue);
            var rawValue = JsonSerializer.Serialize(newValue);
            return new FieldRawValue(rawValue);
        }, predicate);
    }

    public string TransformRaw(string documentContent, Func<FieldKey, FieldRawValue, FieldRawValue> transformRaw, Func<FieldKey, bool> predicate)
    {
        return Transform<FieldRawValue>(documentContent, transformRaw, predicate);
    }

}
