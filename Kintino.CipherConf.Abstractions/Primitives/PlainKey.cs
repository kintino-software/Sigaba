namespace Kintino.CipherConf.Primitives;

public record PlainKey
{
    public byte[] Bytes { get; }
    public PlainKey(PlainData plainData)
    {
        Bytes = plainData.Bytes;
    }
}
