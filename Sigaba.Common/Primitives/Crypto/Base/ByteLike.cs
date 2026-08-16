namespace Sigaba.Primitives.Crypto.Base;

public abstract record ByteLike<TSelf>(byte[] Bytes) : IByteLike where TSelf : class, IByteLike
{

    public string ToBase64() => Convert.ToBase64String(Bytes);
    public static TSelf FromBase64(string base64String)
    {
        var bytes = Convert.FromBase64String(base64String);
        return (TSelf)Activator.CreateInstance(typeof(TSelf), bytes)!;
    }

    public static implicit operator byte[](ByteLike<TSelf> byteLike) => byteLike.Bytes;
}
