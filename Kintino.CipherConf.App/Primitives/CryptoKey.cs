namespace Kintino.CipherConf.App.Primitives;

public record CryptoKey(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
