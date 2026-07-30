namespace Kintino.CipherConf.Primitives.Base;

public abstract record EncryptedByteLike<TSelf>(byte[] Bytes) : ByteLike<TSelf>(Bytes), IEncryptedData where TSelf : IEncryptedData
{
    public static TSelf From<TEncryptedData>(TEncryptedData encryptedData) where TEncryptedData : IEncryptedData
    {
        return (TSelf)Activator.CreateInstance(typeof(TSelf), encryptedData.Bytes)!;
    }
}
