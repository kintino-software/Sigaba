using Sigaba.Crypto.Services;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives;

namespace Sigaba.Crypto;

internal class Cipher(IEnumerable<IVersionedCipher> versionedCiphers) : ICipher
{
    (PublicKey, PrivateKey) ICipher.GenerateKeys()
    {
        var versionedCipher = GetLatestCipher();
        var version = versionedCipher.Version;
        var (publicKey, privateKey) = versionedCipher.GenerateKeys();
        return (publicKey.Tag(version), privateKey.Tag(version));
    }

    PlainData ICipher.DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey)
    {
        var untaggedPrivateKey = privateKey.Untag(out var version);
        var versionedCipher = GetCipherByVersion(version);
        return versionedCipher.DecryptWithKey(encryptedData, untaggedPrivateKey);
    }

    EncryptedData ICipher.EncryptWithKey(PlainData plainData, PublicKey publicKey)
    {
        var untaggedPublicKey = publicKey.Untag(out var version);
        var versionedCipher = GetCipherByVersion(version);
        return versionedCipher.EncryptWithKey(plainData, untaggedPublicKey);
    }

    EncryptedData ICipher.EncryptWithPassword(PlainData plainData, string password)
    {
        throw new NotImplementedException();
    }

    PlainData ICipher.DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        throw new NotImplementedException();
    }

    // helper methods

    private IVersionedCipher GetCipherByVersion(int version)
    {
        var cipher = versionedCiphers.FirstOrDefault(c => c.Version == version);
        return cipher ?? throw new InvalidOperationException($"No asymmetric cipher found for version {version}.");
    }

    private IVersionedCipher GetLatestCipher()
    {
        var latestCipher = versionedCiphers.OrderByDescending(c => c.Version).FirstOrDefault();
        return latestCipher ?? throw new InvalidOperationException("No asymmetric ciphers are available.");
    }

}
