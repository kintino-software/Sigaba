namespace Kintino.CipherConf.Primitives;

public record Nonce
{
    public byte[] Bytes { get; }
    public Nonce(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
}
