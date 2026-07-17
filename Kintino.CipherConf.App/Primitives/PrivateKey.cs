namespace Kintino.CipherConf.App.Primitives;

public record PrivateKey(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
