using Microsoft.Extensions.Logging;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives.Crypto;
using System.Diagnostics.CodeAnalysis;

namespace Sigaba.Crypto;

internal class Cipher(ILogger<Cipher> logger, IEnumerable<IVersionedCipher> versionedCiphers) : ICipher
{
    private readonly Dictionary<byte, IVersionedCipher> versionToCipherMap = versionedCiphers
            .GroupBy(c => c.Version)
            .Select(g => g.Count() > 1
                ? throw new ArgumentException($"Multiple asymmetric ciphers found for version {g.Key}.")
                : g.Single())
            .ToDictionary(c => c.Version);

    private readonly IVersionedCipher latestVersionCipher = versionedCiphers.OrderByDescending(c => c.Version).FirstOrDefault()
            ?? throw new ArgumentException("No asymmetric ciphers are available.");

    private IVersionedCipher GetCipherByVersion(byte version)
    {
        return versionToCipherMap.TryGetValue(version, out var cipher)
            ? cipher
            : throw new InvalidOperationException($"No asymmetric cipher found for version {version}.");
    }

    // interface impl.

    (PublicKey, PrivateKey) ICipher.GenerateKeys()
    {
        CipherLogExtensions.GeneratingKeys(logger, latestVersionCipher.Version);

        var (publicKey, privateKey) = latestVersionCipher.GenerateKeys();

        return (publicKey.Tag(latestVersionCipher.Version), privateKey.Tag(latestVersionCipher.Version));
    }

    PlainData ICipher.DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey)
    {
        var untaggedPrivateKey = privateKey.Untag(out var version);

        logger.DecryptingData(version);

        var versionedCipher = GetCipherByVersion(version);
        return versionedCipher.DecryptWithKey(encryptedData, untaggedPrivateKey);
    }

    EncryptedData ICipher.EncryptWithKey(PlainData plainData, PublicKey publicKey)
    {
        var untaggedPublicKey = publicKey.Untag(out var version);

        logger.EncryptingData(version);

        var versionedCipher = GetCipherByVersion(version);
        return versionedCipher.EncryptWithKey(plainData, untaggedPublicKey);
    }

    EncryptedData ICipher.EncryptWithPassword(PlainData plainData, string password)
    {
        var encryptedData = latestVersionCipher.EncryptWithPassword(plainData, password);

        logger.EncryptingData(latestVersionCipher.Version);

        return encryptedData.Tag(latestVersionCipher.Version);
    }

    PlainData ICipher.DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        var encrypted = encryptedData.Untag(out var version);

        logger.DecryptingData(version);

        var versionedCipher = GetCipherByVersion(version);
        return versionedCipher.DecryptWithPassword(encrypted, password);
    }
}

[ExcludeFromCodeCoverage]
internal static partial class CipherLogExtensions
{
    [LoggerMessage(0, LogLevel.Debug, "Generating keys using algo version {Version}.")]
    public static partial void GeneratingKeys(this ILogger<Cipher> logger, byte version);

    [LoggerMessage(1, LogLevel.Debug, "Encrypting data using algo version {Version}.")]
    public static partial void EncryptingData(this ILogger<Cipher> logger, byte version);

    [LoggerMessage(2, LogLevel.Debug, "Decrypting data using algo version {Version}.")]
    public static partial void DecryptingData(this ILogger<Cipher> logger, byte version);
}