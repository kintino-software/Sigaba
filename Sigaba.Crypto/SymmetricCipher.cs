using Sigaba.Crypto.Services;
using Sigaba.Crypto.Services.Ciphers;
using Sigaba.Primitives;

namespace Sigaba.Crypto;

internal class SymmetricCipher(IEnumerable<IVersionedSymmetricCipher> symmetricCiphers) : ISymmetricCipher
{
    PlainData ISymmetricCipher.Decrypt(PlainKey plainKey, EncryptedData encryptedData, Nonce nonce)
    {
        var untaggedEncryptedData = ByteTagger.Untag(encryptedData, out var version);
        var cipher = GetCipherByVersion(version);
        return cipher.Decrypt(plainKey, untaggedEncryptedData, nonce);
    }

    EncryptedData ISymmetricCipher.Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce)
    {
        var cipher = GetLatestCipher();
        var encryptedData = cipher.Encrypt(plainKey, plainData, nonce);
        return ByteTagger.Tag(encryptedData, cipher.Version);
    }

    PlainKey ISymmetricCipher.GenerateNewKey()
    {
        var cipher = GetLatestCipher();
        var newKey = cipher.GenerateNewKey();
        return new PlainKey(newKey);
    }

    Nonce ISymmetricCipher.GenerateNewNonce()
    {
        var cipher = GetLatestCipher();
        var newNonce = cipher.GenerateNewNonce();
        return new Nonce(newNonce);
    }

    // helper methods

    private IVersionedSymmetricCipher GetCipherByVersion(int version)
    {
        var cipher = symmetricCiphers.FirstOrDefault(c => c.Version == version);
        return cipher ?? throw new InvalidOperationException($"No symmetric cipher found for version {version}.");
    }

    private IVersionedSymmetricCipher GetLatestCipher()
    {
        var latestCipher = symmetricCiphers.OrderByDescending(c => c.Version).FirstOrDefault();
        return latestCipher ?? throw new InvalidOperationException("No symmetric ciphers are available.");
    }
}
