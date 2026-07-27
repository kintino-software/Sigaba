namespace Kintino.CipherConf.Primitives;

public interface IByteLike
{
    byte[] Bytes { get; }
}

public interface IPlainData : IByteLike;
public interface IEncryptedData : IByteLike;


public abstract record ByteLike<TSelf>(byte[] Bytes) : IByteLike where TSelf : IByteLike
{
    public string ToBase64() => Convert.ToBase64String(Bytes);
    public static TSelf FromBase64(string base64String)
    {
        var bytes = Convert.FromBase64String(base64String);
        return (TSelf)Activator.CreateInstance(typeof(TSelf), bytes)!;
    }

    public static implicit operator byte[](ByteLike<TSelf> byteLike) => byteLike.Bytes;
}

public abstract record PlainByteLike<TSelf>(byte[] Bytes) : ByteLike<TSelf>(Bytes), IPlainData where TSelf : IPlainData
{
    public static TSelf From<TPlainData>(TPlainData plainData) where TPlainData : IPlainData
    {
        return (TSelf)Activator.CreateInstance(typeof(TSelf), plainData.Bytes)!;
    }

}

public abstract record EncryptedByteLike<TSelf>(byte[] Bytes) : ByteLike<TSelf>(Bytes), IEncryptedData where TSelf : IEncryptedData
{
    public static TSelf From<TEncryptedData>(TEncryptedData encryptedData) where TEncryptedData : IEncryptedData
    {
        return (TSelf)Activator.CreateInstance(typeof(TSelf), encryptedData.Bytes)!;
    }
}
