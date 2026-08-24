namespace Sigaba.Crypto.Services;

internal class ByteMerger
{
    private readonly Lazy<byte[]> lazyBlob;

    private ByteMerger(Lazy<byte[]> lazyBlob)
    {
        this.lazyBlob = lazyBlob;
    }

    public static ByteMerger FromSplitedData(params byte[][] byteArrays)
    {
        if (byteArrays == null || byteArrays.Length < 2)
        {
            throw new ArgumentException("At least two byte arrays must be provided");
        }

        return new ByteMerger(new(() =>
        {
            var totalLength = byteArrays.Sum(b => b.Length);
            var result = new byte[totalLength];
            int offset = 0;
            foreach (var b in byteArrays)
            {
                b.CopyTo(result, offset);
                offset += b.Length;
            }
            return result;
        }));
    }

    public static ByteMerger FromMergedData(byte[] mergedData)
    {
        if (mergedData.Length < 2)
        {
            throw new ArgumentException("Merged data must have at least two bytes");
        }
        return new ByteMerger(new(() => mergedData));
    }

    public byte[] Merge()
    {
        return lazyBlob.Value;
    }

    public void Split(int lenght1, out byte[] part1, out byte[] part2)
    {
        var data = lazyBlob.Value;
        if (lenght1 < 0 || lenght1 > data.Length)
        {
            throw new ArgumentException("Invalid length for part1");
        }
        part1 = new byte[lenght1];
        part2 = new byte[data.Length - lenght1];
        Buffer.BlockCopy(data, 0, part1, 0, lenght1);
        Buffer.BlockCopy(data, lenght1, part2, 0, data.Length - lenght1);
    }

    public void Split(int length1, int length2, out byte[] part1, out byte[] part2, out byte[] part3)
    {
        var data = lazyBlob.Value;
        if (length1 < 0 || length2 < 0 || length1 + length2 > data.Length)
        {
            throw new ArgumentException("Invalid lengths for parts");
        }
        part1 = new byte[length1];
        part2 = new byte[length2];
        part3 = new byte[data.Length - length1 - length2];
        Buffer.BlockCopy(data, 0, part1, 0, length1);
        Buffer.BlockCopy(data, length1, part2, 0, length2);
        Buffer.BlockCopy(data, length1 + length2, part3, 0, data.Length - length1 - length2);
    }

}
