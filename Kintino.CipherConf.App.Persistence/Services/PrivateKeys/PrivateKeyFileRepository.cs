using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.PrivateKeys;

internal class PrivateKeyFileRepository : IPrivateKeyRepository
{
    private readonly IFileSystem fs = FS.Current;

    async Task<PrivateKey?> IPrivateKeyRepository.LoadAsync(string filePath)
    {
        var privateKeyContent = await fs.File.ReadAllTextAsync(filePath);
        var privateKey = PrivateKey.FromBase64(privateKeyContent);
        return privateKey;
    }

    async Task IPrivateKeyRepository.SaveAsync(PrivateKey privateKey, string filePath)
    {
        var privateKeyContent = privateKey.ToBase64();
        await fs.File.WriteAllTextAsync(filePath, privateKeyContent);
    }

}
