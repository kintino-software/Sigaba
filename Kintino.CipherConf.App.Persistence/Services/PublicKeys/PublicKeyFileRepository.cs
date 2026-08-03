using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.PublicKeys;

internal class PublicKeyFileRepository(IFileSystem fs) : IPublicKeyRepository
{
    public async Task<PublicKey?> LoadAsync(string filePath)
    {
        if (!fs.File.Exists(filePath))
            return null;
        var publicKeyContent = await fs.File.ReadAllTextAsync(filePath);
        var publicKey = PublicKey.FromBase64(publicKeyContent);
        return publicKey;
    }

    public async Task SaveAsync(PublicKey publicKey, string filePath)
    {
        var publicKeyContent = publicKey.ToBase64();
        await fs.File.WriteAllTextAsync(filePath, publicKeyContent);
    }

}
