namespace Kintino.CipherConf.Documents.Models;

internal interface IDocumentModel
{
    static abstract DocumentType DocumentType { get; }
    string Transform<TNewValue>(
        string documentContent,
        Func<IDocumentNode<TNewValue>, TNewValue>? transform,
        Func<IDocumentNode<TNewValue>, string>? transformRaw);
}
