namespace Kintino.CipherConf.Primitives;

public record PrivateKey
{
    public byte[] Bytes { get; }
    public PrivateKey(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
}
