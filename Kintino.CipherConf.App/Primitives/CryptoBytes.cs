namespace Kintino.CipherConf.App.Primitives;

public record CryptoBytes(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
