namespace Kintino.CipherConf.Primitives;

public record PlainKey
{
    public byte[] Bytes { get; }
    public PlainKey(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
    public static implicit operator PlainData(PlainKey plainKey) => new(plainKey.Bytes);
}
