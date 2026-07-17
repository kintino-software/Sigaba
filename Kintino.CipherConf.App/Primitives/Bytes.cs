namespace Kintino.CipherConf.App.Primitives;

public record Bytes // TODO rename to PlainBytes or RawBytes to avoid confusion with CryptoBytes
{
    public byte[] Value { get; init; }
    private readonly Lazy<String64> base64;

    public Bytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            throw new InvalidOperationException("Bytes cannot be null or empty.");
        Value = bytes;
        base64 = new Lazy<String64>(() => new String64(Convert.ToBase64String(Value)));
    }

    public String64 AsBase64()
    {
        return base64.Value;
    }

    public override string ToString()
    {
        return AsBase64().ToString();
    }

    public static implicit operator byte[](Bytes bytes) => bytes.Value;
    public static implicit operator Bytes(byte[] bytes) => new(bytes);
}
