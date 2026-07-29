namespace Kintino.CipherConf.Documents.Models;

internal class DocumentMetadata(Dictionary<int, string> base64Keys)
{
    public IReadOnlyDictionary<int, string> Base64Keys => base64Keys;

    public void AddBase64Key(string key, out int index)
    {
        var n = base64Keys.Count < 1 ? 1 : base64Keys.Max(kvp => kvp.Key) + 1;
        base64Keys[n] = key;
        index = n;
    }

    public void RemoveBase64Key(int index)
    {
        base64Keys.Remove(index);
    }
}
