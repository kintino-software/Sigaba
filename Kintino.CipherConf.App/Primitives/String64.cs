using System.Buffers.Text;

namespace Kintino.CipherConf.App.Primitives;

/// <summary>
/// Represents a validated Base64 encoded string.
/// </summary>
public record String64
{
    public string Value { get; }
    private readonly Lazy<Bytes> bytes;

    public String64(string value)
    {
        if (string.IsNullOrEmpty(value) || !Base64.IsValid(value))
        {
            throw new ArgumentException("Invalid Base64 string.", nameof(value));
        }
        Value = value;
        bytes = new Lazy<Bytes>(() => Convert.FromBase64String(Value));
    }

    public Bytes AsBytes()
    {
        return bytes.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
