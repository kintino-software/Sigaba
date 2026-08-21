using Konscious.Security.Cryptography;
using Sigaba.Primitives.Crypto;
using System.Security.Cryptography;
using System.Text;

namespace Sigaba.Crypto.Services.Ciphers.V1;

using NSec = NSec.Cryptography;

/// <summary>
/// Helper class for CipherV1.
/// Uses Argon2id for key derivation and ChaCha20-Poly1305 for encryption/decryption.
/// </summary>
internal static class PasswordAlgoV1
{
    private static readonly NSec.AeadAlgorithm aeadAlgorithm = NSec.AeadAlgorithm.ChaCha20Poly1305;

    const int SaltSizeInBytes = 16; // 128 bits
    const int KeySizeInBytes = 32; // 256 bits
    const int Argon2Iterations = 4;
    const int ArgonMemorySize = 65536; // 64 MB
    const int ArgonParallelism = 4;

    public static EncryptedData Encrypt(IPlainData plainData, string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSizeInBytes);
        var key = DeriveKey(password, salt);

        // Import key into NSec
        using var encryptionKey = NSec.Key.Import(aeadAlgorithm, key, NSec.KeyBlobFormat.RawSymmetricKey);

        // ChaCha20-Poly1305 uses a random nonce (must never repeat with same key)
        var nonce = RandomNumberGenerator.GetBytes(aeadAlgorithm.NonceSize);

        // Encrypt (ciphertext includes authentication tag)
        var ciphertext = aeadAlgorithm.Encrypt(encryptionKey, nonce, associatedData: null, plainData.Bytes);

        var merged = ByteMerger
            .FromSplitedData(salt, nonce, ciphertext)
            .Merge();
        return new EncryptedData(merged);
    }

    public static PlainData Decrypt(IEncryptedData encryptedData, string password)
    {
        ByteMerger
            .FromMergedData(encryptedData.Bytes)
            .Split(SaltSizeInBytes, aeadAlgorithm.NonceSize, out var salt, out var nonce, out var ciphertext);

        // Derive key using Argon2id
        var key = DeriveKey(password, salt);
        using var decryptionKey = NSec.Key.Import(aeadAlgorithm, key, NSec.KeyBlobFormat.RawSymmetricKey);

        // Decrypt and verify authentication tag
        var plaintext = aeadAlgorithm.Decrypt(decryptionKey, nonce, associatedData: null, ciphertext)
            ?? throw new CryptographicException("Decryption failed: wrong password or data tampered");

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
}
