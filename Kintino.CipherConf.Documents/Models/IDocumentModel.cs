namespace Kintino.CipherConf.Documents.Models;

internal interface IDocumentModel
{
    void Deserialize(string content);
    IEnumerable<IDocumentNode> GetNodes();
    void UpdateNodeContent(IDocumentNode node, string newContent);
    string Serialize();
}
