namespace Kintino.CipherConf.App.Primitives;

public record Nonce(Bytes Bytes)
{
    public override string ToString()
    {
        return Bytes.ToString();
    }
}
