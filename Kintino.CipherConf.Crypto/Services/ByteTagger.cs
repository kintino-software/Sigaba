using Kintino.CipherConf.Primitives.Base;

namespace Kintino.CipherConf.Crypto.Services;

internal static class ByteTagger
{
    public static TByteLike Tag<TByteLike>(TByteLike input, byte tag) where TByteLike : IByteLike
    {
        var taggedBytes = new byte[input.Bytes.Length + 1];

        taggedBytes[0] = tag;
        Buffer.BlockCopy(input.Bytes, 0, taggedBytes, 1, input.Bytes.Length);
        return (TByteLike)Activator.CreateInstance(typeof(TByteLike), taggedBytes)!;
    }

    public static TByteLike Untag<TByteLike>(TByteLike input, out byte tag) where TByteLike : IByteLike
    {
        if (input.Bytes.Length < 1)
        {
            throw new InvalidOperationException("Byte array is too short to contain a tag.");
        }
        tag = input.Bytes[0];
        var untaggedBytes = new byte[input.Bytes.Length - 1];
        Buffer.BlockCopy(input.Bytes, 1, untaggedBytes, 0, untaggedBytes.Length);
        var untagged = (TByteLike)Activator.CreateInstance(typeof(TByteLike), untaggedBytes)!;
        return untagged;
    }
}
