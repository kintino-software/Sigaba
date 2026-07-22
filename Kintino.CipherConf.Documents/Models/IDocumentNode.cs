namespace Kintino.CipherConf.Documents.Models;

internal interface IDocumentNode<TValue>
{
    public string Key { get; }
    public TValue Content { get; }
    public string RawContent { get; }

}
