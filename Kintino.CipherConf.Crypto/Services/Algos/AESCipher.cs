using Kintino.CipherConf.Crypto.Primitives;
using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.Services.Algos;

internal static class AESCipher
{
    public const int TagSize = 16; // 128 bits

    public static byte[] Encrypt(byte[] plainData, byte[] key, byte[] nonce)
    {
        if (plainData == null || plainData.Length == 0)
            throw new CryptoException("Cannot encrypt null or empty data.");

        byte[] cipher = new byte[plainData.Length];
        byte[] tag = new byte[TagSize];

        try
        {
            using (var aesGcm = new AesGcm(key, tag.Length))
                aesGcm.Encrypt(nonce, plainData, cipher, tag);

            return MergeTagAndData(tag, cipher);
        }
        catch (Exception ex) when (ex is CryptographicException)
        {
            throw new CryptoException("Could not encrypt data.", ex);
        }

    }

    public static byte[] Decrypt(byte[] encryptedData, byte[] key, byte[] nonce)
    {
        SplitTagAndData(encryptedData, out byte[] tag, out byte[] data);

        byte[] plainData = new byte[data.Length];

        try
        {
            using (var aesGcm = new AesGcm(key, tag.Length))
                aesGcm.Decrypt(nonce, data, tag, plainData);
            return plainData;
        }
        catch (Exception ex) when (ex is CryptographicException || ex is AuthenticationTagMismatchException)
        {
            throw new CryptoException("Decryption failed.", ex);
        }

    }

    private static byte[] MergeTagAndData(byte[] tag, byte[] data)
    {
        byte[] result = new byte[tag.Length + data.Length];
        Buffer.BlockCopy(tag, 0, result, 0, tag.Length);
        Buffer.BlockCopy(data, 0, result, tag.Length, data.Length);
        return result;
    }

    private static void SplitTagAndData(byte[] combined, out byte[] tag, out byte[] data)
    {
        if (combined.Length < TagSize)
            throw new ArgumentException("Combined array is too short to contain a valid tag.", nameof(combined));

        tag = new byte[TagSize];
        data = new byte[combined.Length - TagSize];

        Buffer.BlockCopy(combined, 0, tag, 0, TagSize);
        Buffer.BlockCopy(combined, TagSize, data, 0, data.Length);
    }

}
