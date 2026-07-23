namespace Kintino.CipherConf.Primitives;

public record PrivateKey
{
    public byte[] Bytes { get; }
    public PrivateKey(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
    public static implicit operator PlainData(PrivateKey privateKey) => new(privateKey.Bytes);
}
