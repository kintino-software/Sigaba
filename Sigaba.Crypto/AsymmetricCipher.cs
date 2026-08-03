using Sigaba.Crypto.Services;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives;

namespace Sigaba.Crypto;

internal class AsymmetricCipher(IEnumerable<IVersionedAsymmetricCipher> asymmetricCiphers) : IAsymmetricCipher
{
    (PublicKey PublicKey, PrivateKey PrivateKey) IAsymmetricCipher.CreateNewKeyPair()
    {
        var cipher = GetLatestCipher();
        return cipher.CreateNewKeyPair();
    }

    PlainKey IAsymmetricCipher.Decrypt(EncryptedKey encryptedData, PrivateKey privateKey)
    {
        var untaggedEncryptedData = ByteTagger.Untag(encryptedData, out var version);
        var cipher = GetCipherByVersion(version);
        return cipher.Decrypt(untaggedEncryptedData, privateKey);
    }

    EncryptedKey IAsymmetricCipher.Encrypt(PlainKey plainData, PublicKey publicKey)
    {
        var cipher = GetLatestCipher();
        var encryptedData = cipher.Encrypt(plainData, publicKey);
        return ByteTagger.Tag(encryptedData, cipher.Version);
    }

    // helper methods

    private IVersionedAsymmetricCipher GetCipherByVersion(int version)
    {
        var cipher = asymmetricCiphers.FirstOrDefault(c => c.Version == version);
        return cipher ?? throw new InvalidOperationException($"No asymmetric cipher found for version {version}.");
    }

    private IVersionedAsymmetricCipher GetLatestCipher()
    {
        var latestCipher = asymmetricCiphers.OrderByDescending(c => c.Version).FirstOrDefault();
        return latestCipher ?? throw new InvalidOperationException("No asymmetric ciphers are available.");
    }

}
