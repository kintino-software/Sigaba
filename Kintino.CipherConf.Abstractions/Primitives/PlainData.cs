namespace Kintino.CipherConf.Primitives;

public record PlainData(byte[] Bytes)
{
    public static implicit operator byte[](PlainData data) => data.Bytes;
    public static implicit operator PlainData(byte[] bytes) => new(bytes);
}