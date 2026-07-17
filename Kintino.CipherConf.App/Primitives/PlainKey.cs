namespace Kintino.CipherConf.App.Primitives;

public record PlainKey(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
