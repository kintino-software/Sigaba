using Sigaba.Primitives;

namespace Sigaba.Crypto.Services;

internal static class ByteTaggerExtensions
{
    public static byte[] Tag(this byte[] input, byte tag)
    {
        var taggedBytes = new byte[input.Length + 1];

        taggedBytes[0] = tag;
        Buffer.BlockCopy(input, 0, taggedBytes, 1, input.Length);
        return taggedBytes;
    }

    public static byte[] Untag(this byte[] input, out byte tag)
    {
        if (input.Length < 1)
        {
            throw new InvalidOperationException("Byte array is too short to contain a tag.");
        }
        tag = input[0];
        var untaggedBytes = new byte[input.Length - 1];
        Buffer.BlockCopy(input, 1, untaggedBytes, 0, untaggedBytes.Length);
        return untaggedBytes;
    }

    public static EncryptedData Tag(this EncryptedData data, byte tag) => new(Tag(data.Bytes, tag));
    public static EncryptedData Untag(this EncryptedData data, out byte tag) => new(Untag(data.Bytes, out tag));

    public static PublicKey Tag(this PublicKey data, byte tag) => new(Tag(data.Bytes, tag));
    public static PublicKey Untag(this PublicKey data, out byte tag) => new(Untag(data.Bytes, out tag));

    public static PrivateKey Tag(this PrivateKey data, byte tag) => new(Tag(data.Bytes, tag));
    public static PrivateKey Untag(this PrivateKey data, out byte tag) => new(Untag(data.Bytes, out tag));



}
