using Konscious.Security.Cryptography;
using Sigaba.Primitives;
using System.Security.Cryptography;
using System.Text;

namespace Sigaba.Crypto.Services.Ciphers.V1;
/// <summary>
/// Helper class for CipherV1.
/// Uses argon2id for key derivation and AES-GCM for encryption/decryption.
/// </summary>
internal static class PasswordAlgoV1
{
    const int SaltSizeInBytes = 16; // 128 bits
    const int NonceSizeInBytes = 12; // 96 bits
    const int TagSizeInBytes = 16; // 128 bits
    const int KeySizeInBytes = 32; // 256 bits
    const int Argon2Iterations = 4;
    const int ArgonMemorySize = 65536; // 64 MB
    const int ArgonParallelism = 4;

    public static EncryptedData Encrypt(PlainData plainData, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeInBytes);
        var key = DeriveKey(password, salt);
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeInBytes);

        var ciphertext = new byte[plainData.Bytes.Length];
        var tag = new byte[TagSizeInBytes];

        using var aes = new AesGcm(key, TagSizeInBytes);
        aes.Encrypt(nonce, plainData.Bytes, ciphertext, tag);

        return new EncryptedData(MergeSaltNonceTagCiphertext(salt, nonce, tag, ciphertext));
    }

    public static PlainData Decrypt(EncryptedData encryptedData, string password)
    {
        SplitSaltNonceTagCiphertext(encryptedData.Bytes, out var salt, out var nonce, out var tag, out var ciphertext);

        var key = DeriveKey(password, salt);
        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, TagSizeInBytes);
        aes.Decrypt(nonce, ciphertext, tag, plaintext); // throws if password wrong / tampered

        return new PlainData(plaintext);
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = ArgonParallelism,
            MemorySize = ArgonMemorySize,
            Iterations = Argon2Iterations
        };
        return argon2.GetBytes(KeySizeInBytes);
    }

    private static byte[] MergeSaltNonceTagCiphertext(byte[] salt, byte[] nonce, byte[] tag, byte[] ciphertext)
    {
        var result = new byte[salt.Length + nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
        Buffer.BlockCopy(nonce, 0, result, salt.Length, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, salt.Length + nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, result, salt.Length + nonce.Length + tag.Length, ciphertext.Length);
        return result;
    }

    private static void SplitSaltNonceTagCiphertext(byte[] mergedData, out byte[] salt, out byte[] nonce, out byte[] tag, out byte[] ciphertext)
    {
        salt = mergedData[..SaltSizeInBytes];
        nonce = mergedData[SaltSizeInBytes..(SaltSizeInBytes + NonceSizeInBytes)];
        tag = mergedData[(SaltSizeInBytes + NonceSizeInBytes)..(SaltSizeInBytes + NonceSizeInBytes + TagSizeInBytes)];
        ciphertext = mergedData[(SaltSizeInBytes + NonceSizeInBytes + TagSizeInBytes)..];
    }

}
