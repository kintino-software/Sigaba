namespace Kintino.CipherConf.Documents.Models;

internal interface IDocumentModel
{
    static abstract DocumentType DocumentType { get; }
    string Transform<TNewValue>(string documentContent, Func<FieldKey, FieldRawValue, TNewValue> transform, Func<FieldKey, bool> predicate);
    string TransformRaw(string documentContent, Func<FieldKey, FieldRawValue, FieldRawValue> transformRaw, Func<FieldKey, bool> predicate);
}
