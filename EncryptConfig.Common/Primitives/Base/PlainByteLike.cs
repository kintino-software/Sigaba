using Kintino.CipherConf.Primitives.Base;

namespace Kintino.CipherConf.Primitives.Base;

public abstract record PlainByteLike<TSelf>(byte[] Bytes) : ByteLike<TSelf>(Bytes), IPlainData where TSelf : IPlainData
{
    public static TSelf From<TPlainData>(TPlainData plainData) where TPlainData : IPlainData
    {
        return (TSelf)Activator.CreateInstance(typeof(TSelf), plainData.Bytes)!;
    }

}
