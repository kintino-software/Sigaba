using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives.Crypto;

namespace Sigaba.Crypto;

internal partial class Cipher
{
    private readonly Dictionary<byte, IVersionedCipher> versionToCipherMap;
    private readonly IVersionedCipher latestVersionCipher;

    public Cipher(IEnumerable<IVersionedCipher> versionedCiphers)
    {
        versionToCipherMap = versionedCiphers
            .GroupBy(c => c.Version)
            .Select(g => g.Count() > 1
                ? throw new ArgumentException($"Multiple asymmetric ciphers found for version {g.Key}.")
                : g.Single())
            .ToDictionary(c => c.Version);

        latestVersionCipher = versionedCiphers.OrderByDescending(c => c.Version).FirstOrDefault()
            ?? throw new ArgumentException("No asymmetric ciphers are available.");
    }

    private IVersionedCipher GetCipherByVersion(byte version)
    {
        return versionToCipherMap.TryGetValue(version, out var cipher)
            ? cipher
            : throw new InvalidOperationException($"No asymmetric cipher found for version {version}.");
    }

    private IVersionedCipher GetLatestCipher() => latestVersionCipher;
}

internal partial class Cipher : ICipher
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
        var versionedCipher = GetLatestCipher();
        var encryptedData = versionedCipher.EncryptWithPassword(plainData, password);
        return encryptedData.Tag(versionedCipher.Version);
    }

    PlainData ICipher.DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        var encrypted = encryptedData.Untag(out var version);
        var versionedCipher = GetCipherByVersion(version);
        return versionedCipher.DecryptWithPassword(encrypted, password);
    }
}
