namespace Kintino.CipherConf.Primitives;

public record Nonce
{
    public byte[] Bytes { get; }
    public Nonce(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
    public static implicit operator PlainData(Nonce nonce) => new(nonce.Bytes);
}
