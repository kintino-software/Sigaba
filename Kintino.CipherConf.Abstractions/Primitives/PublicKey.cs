namespace Kintino.CipherConf.Primitives;

public record PublicKey
{
    public byte[] Bytes { get; }
    public PublicKey(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
    public static implicit operator PlainData(PublicKey publicKey) => new(publicKey.Bytes);
}
