using Sigaba.Primitives;
using System.Security.Cryptography;

namespace Sigaba.Crypto.Services.Ciphers.V1;

/// <summary>
/// AESGcm implementation of symmetric cipher. 
/// <br/>
/// Tag size is fixed at 16 bytes (128 bits) for this implementation.
/// <br/>
/// Key size is fixed at 32 bytes (256 bits) for this implementation.
/// </summary>
internal class SymmetricCipherV1 : IVersionedSymmetricCipher
{
    public const int TagSizeInBytes = 16; // 128 bits
    public const int KeySizeInBytes = 32; // 256 bits
    public const int NonceSizeInBytes = 12; // 96 bits

    byte IVersionedSymmetricCipher.Version { get; } = 1;

    PlainKey ISymmetricCipher.GenerateNewKey()
    {
        return new PlainKey(RNG.GetBytes(KeySizeInBytes));
    }

    Nonce ISymmetricCipher.GenerateNewNonce()
    {
        return new Nonce(RNG.GetBytes(NonceSizeInBytes));
    }

    EncryptedData ISymmetricCipher.Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce)
    {

        if (plainData == null || plainData.Bytes.Length == 0)
            throw new ArgumentException("Cannot encrypt null or empty data.", nameof(plainData));

        byte[] encryptedBytes = new byte[plainData.Bytes.Length];
        byte[] tag = new byte[TagSizeInBytes];

        try
        {
            using (var aesGcm = new AesGcm(plainKey.Bytes, tag.Length))
                aesGcm.Encrypt(
                    nonce: nonce,
                    plaintext: plainData,
                    ciphertext: encryptedBytes,
                    tag: tag);

            var mergedBytes = MergeTagAndData(tag, encryptedBytes);
            return new EncryptedData(mergedBytes);
        }
        catch (Exception ex) when (ex is CryptographicException)
        {
            throw new Exception("Could not encrypt data.", ex);
        }
    }

    PlainData ISymmetricCipher.Decrypt(PlainKey plainKey, EncryptedData encryptedData, Nonce nonce)
    {
        SplitTagAndData(encryptedData, out byte[] tag, out byte[] data, TagSizeInBytes);

        byte[] plainData = new byte[data.Length];

        try
        {
            using (var aesGcm = new AesGcm(plainKey, tag.Length))
                aesGcm.Decrypt(nonce, data, tag, plainData);

            return new PlainData(plainData);
        }
        catch (Exception ex) when (ex is CryptographicException || ex is AuthenticationTagMismatchException)
        {
            throw new Exception("Decryption failed.", ex);
        }
    }

    private static byte[] MergeTagAndData(byte[] tag, byte[] data)
    {
        byte[] result = new byte[tag.Length + data.Length];
        Buffer.BlockCopy(tag, 0, result, 0, tag.Length);
        Buffer.BlockCopy(data, 0, result, tag.Length, data.Length);
        return result;
    }

    private static void SplitTagAndData(byte[] combined, out byte[] tag, out byte[] data, int tagSize)
    {
        if (combined.Length < tagSize)
            throw new ArgumentException("Combined array is too short to contain a valid tag.", nameof(combined));

        tag = new byte[tagSize];
        data = new byte[combined.Length - tagSize];

        Buffer.BlockCopy(combined, 0, tag, 0, tagSize);
        Buffer.BlockCopy(combined, tagSize, data, 0, data.Length);
    }
}
