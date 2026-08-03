using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal class PrivateKeyFileRepository(IFileSystem fs) : IPrivateKeyRepository
{
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
