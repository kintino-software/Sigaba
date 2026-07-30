namespace Kintino.CipherConf.Crypto.Services;

internal class CipherFactory(
    IEnumerable<ISymmetricCipher> symmetricCiphers,
    IEnumerable<IAsymmetricCipher> asymmetricCiphers)
    : ICipherFactory
{
    IAsymmetricCipher ICipherFactory.GetLatestAsymmetricCipher()
    {
        var latestVersion = asymmetricCiphers.Max(x => x.Version);
        return asymmetricCiphers.FirstOrDefault(c => c.Version == latestVersion)
            ?? throw new NotImplementedException($"Asymmetric cipher version {latestVersion} not found.");
    }

    ISymmetricCipher ICipherFactory.GetLatestSymmetricCipher()
    {
        var latestVersion = symmetricCiphers.Max(x => x.Version);
        return symmetricCiphers.FirstOrDefault(c => c.Version == latestVersion)
            ?? throw new NotImplementedException($"Symmetric cipher version {latestVersion} not found.");
    }

    IAsymmetricCipher ICipherFactory.GetAsymmetricCipher(int version)
    {
        return asymmetricCiphers.FirstOrDefault(c => c.Version == version)
            ?? throw new NotImplementedException($"Asymmetric cipher version {version} not found.");
    }

    ISymmetricCipher ICipherFactory.GetSymmetricCipher(int version)
    {
        return symmetricCiphers.FirstOrDefault(c => c.Version == version)
            ?? throw new NotImplementedException($"Symmetric cipher version {version} not found.");
    }
}
