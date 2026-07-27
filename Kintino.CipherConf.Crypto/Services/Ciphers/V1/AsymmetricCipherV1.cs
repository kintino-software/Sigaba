using Kintino.CipherConf.Primitives;
using System.Security.Cryptography;

namespace Kintino.CipherConf.Crypto.Services.Ciphers.V1;

/// <summary>
/// RSA implementation of asymmetric cipher.
/// <br/>
/// Key size is fixed at 384 bytes for this implementation.
/// </summary>
internal class AsymmetricCipherV1 : IAsymmetricCipher
{
    public const int RsaKeySizeInBytes = 384; // 3072 bits is the recommended size at july-2026
    public static readonly RSAEncryptionPadding Padding = RSAEncryptionPadding.OaepSHA256;

    int IAsymmetricCipher.Version { get; } = 1;

    (PublicKey PublicKey, PrivateKey PrivateKey) IAsymmetricCipher.CreateNewKeyPair()
    {
        using var rsa = RSA.Create(RsaKeySizeInBytes * 8); // input is in bits, so multiply by 8

        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var privateKeyBytes = rsa.ExportRSAPrivateKey();

        return (new PublicKey(publicKeyBytes), new PrivateKey(privateKeyBytes));
    }

    EncryptedKey IAsymmetricCipher.Encrypt(PlainKey plainData, PublicKey publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey.Bytes, out _);
        var encryptedBytes = rsa.Encrypt(plainData.Bytes, Padding);
        return new EncryptedKey(encryptedBytes);
    }

    PlainKey IAsymmetricCipher.Decrypt(EncryptedKey encryptedData, PrivateKey privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey.Bytes, out _);
        var plainKeyBytes = rsa.Decrypt(encryptedData.Bytes, Padding);
        return new PlainKey(plainKeyBytes);
    }
}
