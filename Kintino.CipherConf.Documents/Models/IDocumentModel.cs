namespace Kintino.CipherConf.Documents.Models;

internal interface IDocumentModel
{
    static abstract DocumentType DocumentType { get; }
    string Transform(
        string documentContent,
        Func<IDocumentNode, string> transform);
}
