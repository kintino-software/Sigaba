namespace Kintino.CipherConf.App.Primitives;

public record PlainBytes(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
