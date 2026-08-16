using Sigaba.Primitives.Crypto;
using System.Security.Cryptography;

namespace Sigaba.Crypto.Services.Ciphers.V1;

using NSec = NSec.Cryptography;

internal static class AsymmetricAlgoV1
{
    private readonly static NSec.KeyAgreementAlgorithm keyAgreementAlgorithm = NSec.KeyAgreementAlgorithm.X25519;
    private readonly static NSec.AeadAlgorithm aeadAlgorithm = NSec.AeadAlgorithm.ChaCha20Poly1305;
    private readonly static byte[] hkdfInfo = "Sigaba.Crypto.V1.X25519.ChaCha20Poly1305"u8.ToArray();

    public static (PublicKey, PrivateKey) GenerateKeys()
    {
        using var key = NSec.Key.Create(keyAgreementAlgorithm, new NSec.KeyCreationParameters
        {
            ExportPolicy = NSec.KeyExportPolicies.AllowPlaintextExport
        });
        var privateKeyBytes = key.Export(NSec.KeyBlobFormat.RawPrivateKey);
        var publicKeyBytes = key.PublicKey.Export(NSec.KeyBlobFormat.RawPublicKey);
        return (new PublicKey(publicKeyBytes), new PrivateKey(privateKeyBytes));
    }

    public static EncryptedData Encrypt(IPlainData plainData, PublicKey publicKey)
    {
        // Import recipient's public key
        var recipientPublicKey = NSec.PublicKey.Import(keyAgreementAlgorithm, publicKey.Bytes, NSec.KeyBlobFormat.RawPublicKey);

        // Generate ephemeral key pair
        using var ephemeralKey = NSec.Key.Create(keyAgreementAlgorithm);
        var ephemeralPublicKeyBytes = ephemeralKey.PublicKey.Export(NSec.KeyBlobFormat.RawPublicKey);

        // Perform X25519 key agreement to get shared secret
        using var sharedSecret = keyAgreementAlgorithm.Agree(ephemeralKey, recipientPublicKey)
            ?? throw new CryptographicException("Key agreement failed");

        // Derive encryption key using HKDF from shared secret
        using var encryptionKey = NSec.KeyDerivationAlgorithm.HkdfSha256.DeriveKey(
            sharedSecret,
            salt: null,
            info: hkdfInfo,
            aeadAlgorithm);

        // Encrypt plaintext using ChaCha20-Poly1305, with ephemeral public key as AAD
        // Using zero nonce is safe here because we derive a unique key for each message
        var nonce = new byte[aeadAlgorithm.NonceSize];
        var ciphertext = aeadAlgorithm.Encrypt(encryptionKey, nonce, ephemeralPublicKeyBytes, plainData.Bytes);

        // Format: [ephemeralPublicKey (32 bytes)][ciphertext (includes tag)]
        var result = new byte[ephemeralPublicKeyBytes.Length + ciphertext.Length];
        Buffer.BlockCopy(ephemeralPublicKeyBytes, 0, result, 0, ephemeralPublicKeyBytes.Length);
        Buffer.BlockCopy(ciphertext, 0, result, ephemeralPublicKeyBytes.Length, ciphertext.Length);

        return new EncryptedData(result);
    }

    public static PlainData Decrypt(IEncryptedData encryptedData, PrivateKey privateKey)
    {
        // Format: [ephemeralPublicKey (32 bytes)][ciphertext (includes tag)]
        const int ephemeralPublicKeySize = 32; // X25519 public key size

        if (encryptedData.Bytes.Length < ephemeralPublicKeySize)
        {
            throw new CryptographicException("Invalid encrypted data format: too short");
        }

        // Extract ephemeral public key
        var ephemeralPublicKeyBytes = new byte[ephemeralPublicKeySize];
        Buffer.BlockCopy(encryptedData.Bytes, 0, ephemeralPublicKeyBytes, 0, ephemeralPublicKeySize);
        var ephemeralPublicKey = NSec.PublicKey.Import(keyAgreementAlgorithm, ephemeralPublicKeyBytes, NSec.KeyBlobFormat.RawPublicKey);

        // Extract ciphertext
        var ciphertext = new byte[encryptedData.Bytes.Length - ephemeralPublicKeySize];
        Buffer.BlockCopy(encryptedData.Bytes, ephemeralPublicKeySize, ciphertext, 0, ciphertext.Length);

        // Import recipient's private key
        using var recipientPrivateKey = NSec.Key.Import(keyAgreementAlgorithm, privateKey.Bytes, NSec.KeyBlobFormat.RawPrivateKey);

        // Perform X25519 key agreement to get shared secret
        using var sharedSecret = keyAgreementAlgorithm.Agree(recipientPrivateKey, ephemeralPublicKey)
            ?? throw new CryptographicException("Key agreement failed");

        // Derive decryption key using HKDF from shared secret
        using var decryptionKey = NSec.KeyDerivationAlgorithm.HkdfSha256.DeriveKey(
            sharedSecret,
            salt: null,
            info: hkdfInfo,
            aeadAlgorithm);

        // Decrypt ciphertext using ChaCha20-Poly1305, verify ephemeral public key AAD
        // Using zero nonce is safe here because we derive a unique key for each message
        var nonce = new byte[aeadAlgorithm.NonceSize];
        var plaintext = aeadAlgorithm.Decrypt(decryptionKey, nonce, ephemeralPublicKeyBytes, ciphertext)
            ?? throw new CryptographicException("Decryption failed: authentication tag verification failed");

        return new PlainData(plaintext);
    }

}
