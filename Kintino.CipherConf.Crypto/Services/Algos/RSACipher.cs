using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.Services.Algos;

internal static class RSACipher
{
    private const int RsaKeySizeInBits = 3072; // 3072 bits is the recommended size at july-2026

    public static void CreateNewKeyPair(out byte[] publicKey, out byte[] privateKey)
    {
        using var rsa = RSA.Create(RsaKeySizeInBits);

        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var privateKeyBytes = rsa.ExportRSAPrivateKey();

        publicKey = publicKeyBytes;
        privateKey = privateKeyBytes;
    }

    public static byte[] Encrypt(byte[] plainData, byte[] publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.Encrypt(plainData, RSAEncryptionPadding.OaepSHA256);
    }

    public static byte[] Decrypt(byte[] cipher, byte[] privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        return rsa.Decrypt(cipher, RSAEncryptionPadding.OaepSHA256);
    }

}
