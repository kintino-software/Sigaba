using System.Buffers.Text;

namespace Sigaba.Primitives.Crypto.Base;

public abstract record ByteLike<TSelf>(byte[] Bytes) : IByteLike where TSelf : class, IByteLike
{

    public string ToBase64()
    {
        return Base64Url.EncodeToString(Bytes);
    }

    public static TSelf FromBase64(string base64String)
    {
        var bytes = Base64Url.DecodeFromChars(base64String);
        return (TSelf)Activator.CreateInstance(typeof(TSelf), bytes)!;
    }

    public static implicit operator byte[](ByteLike<TSelf> byteLike) => byteLike.Bytes;
}
