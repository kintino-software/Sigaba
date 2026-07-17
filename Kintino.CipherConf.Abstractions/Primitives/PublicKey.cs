namespace Kintino.CipherConf.Primitives;

public record PublicKey
{
    public byte[] Bytes { get; }
    public PublicKey(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
}
