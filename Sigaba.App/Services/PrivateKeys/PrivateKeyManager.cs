using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal partial class PrivateKeyManager(IFileSystem fs) : IPrivateKeyManager
{
    async Task<PrivateKey?> IPrivateKeyManager.LoadAsync(string filePath)
    {
        if (!fs.File.Exists(filePath))
            return null;
        var privateKeyContent = await fs.File.ReadAllTextAsync(filePath);
        var privateKey = PrivateKey.FromBase64(privateKeyContent);
        return privateKey;
    }

    async Task IPrivateKeyManager.SaveAsync(PrivateKey privateKey, string filePath)
    {
        var directory = fs.Path.GetDirectoryName(filePath)
            ?? throw new InvalidOperationException($"Cannot get directory name from file path: {filePath}");
        fs.CreateFolderIfNotExists(directory);
        var privateKeyContent = privateKey.ToBase64();
        await fs.File.WriteAllTextAsync(filePath, privateKeyContent);
    }



}
