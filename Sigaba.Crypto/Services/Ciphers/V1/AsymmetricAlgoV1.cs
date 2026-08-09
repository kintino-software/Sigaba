using Sigaba.Primitives;
using System.Security.Cryptography;

namespace Sigaba.Crypto.Services.Ciphers.V1;

internal static class AsymmetricAlgoV1
{
    private static ECCurve curve = ECCurve.NamedCurves.nistP256;
    private const int NonceSizeInBytes = 12; // 96 bits
    private const int TagSizeInBytes = 16; // 128 bits
    private const int AesKeySizeInBytes = 32; // 256 bits

    public static (PublicKey, PrivateKey) GenerateKeys()
    {
        using var ecc = ECDiffieHellman.Create(curve);
        var privateKeyBytes = ecc.ExportPkcs8PrivateKey();
        var publicKeyBytes = ecc.ExportSubjectPublicKeyInfo();
        return (new PublicKey(publicKeyBytes), new PrivateKey(privateKeyBytes));
    }

    public static EncryptedData Encrypt(PlainData plainData, PublicKey publicKey)
    {
        using var recipientECDHPublicKey = ECDiffieHellman.Create();
        recipientECDHPublicKey.ImportSubjectPublicKeyInfo(publicKey.Bytes, out _);

        using var ephemeral = ECDiffieHellman.Create(curve);
        var sharedSecret = ephemeral.DeriveRawSecretAgreement(recipientECDHPublicKey.PublicKey);
        var aesKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, AesKeySizeInBytes);

        var encryptedData = new byte[plainData.Bytes.Length];
        var nonce = new byte[NonceSizeInBytes];
        var tag = new byte[TagSizeInBytes];
        using var aes = new AesGcm(aesKey, tag.Length);
        aes.Encrypt(nonce, plainData.Bytes, encryptedData, tag);

        return new EncryptedData(MergeDataNonceTagPublicKey(encryptedData, nonce, tag, ephemeral.ExportSubjectPublicKeyInfo()));
    }

    public static PlainData Decrypt(EncryptedData encryptedData, PrivateKey privateKey)
    {
        SplitDataNonceTagPublicKey(encryptedData.Bytes, out var data, out var nonce, out var tag, out var publicKeyBytes);

        using var ephemeral = ECDiffieHellman.Create();
        ephemeral.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        using var recipientECDHPrivateKey = ECDiffieHellman.Create();
        recipientECDHPrivateKey.ImportPkcs8PrivateKey(privateKey.Bytes, out _);
        var sharedSecret = recipientECDHPrivateKey.DeriveRawSecretAgreement(ephemeral.PublicKey);

        var aesKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, AesKeySizeInBytes);
        var plainDataBytes = new byte[data.Length];
        using var aes = new AesGcm(aesKey, tag.Length);
        aes.Decrypt(nonce, data, tag, plainDataBytes);

        return new PlainData(plainDataBytes);
    }

    private static byte[] MergeDataNonceTagPublicKey(byte[] data, byte[] nonce, byte[] tag, byte[] publicKey)
    {
        var result = new byte[data.Length + nonce.Length + tag.Length + publicKey.Length];
        Buffer.BlockCopy(data, 0, result, 0, data.Length);
        Buffer.BlockCopy(nonce, 0, result, data.Length, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, data.Length + nonce.Length, tag.Length);
        Buffer.BlockCopy(publicKey, 0, result, data.Length + nonce.Length + tag.Length, publicKey.Length);
        return result;
    }

    private static void SplitDataNonceTagPublicKey(byte[] mergedData, out byte[] data, out byte[] nonce, out byte[] tag, out byte[] publicKey)
    {
        int dataLength = mergedData.Length - NonceSizeInBytes - TagSizeInBytes - 91; // 91 is the length of the public key in bytes
        data = new byte[dataLength];
        nonce = new byte[NonceSizeInBytes];
        tag = new byte[TagSizeInBytes];
        publicKey = new byte[91];
        Buffer.BlockCopy(mergedData, 0, data, 0, dataLength);
        Buffer.BlockCopy(mergedData, dataLength, nonce, 0, NonceSizeInBytes);
        Buffer.BlockCopy(mergedData, dataLength + NonceSizeInBytes, tag, 0, TagSizeInBytes);
        Buffer.BlockCopy(mergedData, dataLength + NonceSizeInBytes + TagSizeInBytes, publicKey, 0, 91);
    }
}
