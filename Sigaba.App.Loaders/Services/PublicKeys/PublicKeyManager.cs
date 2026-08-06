using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PublicKeys;

internal partial class PublicKeyManager(IFileSystem fs) : IPublicKeyManager
{
    private string Cwd => fs.Directory.GetCurrentDirectory();
    private string FilePath => fs.Path.Combine(Cwd, Constants.PublicKeyFileName);
    private bool FileExists() => fs.File.Exists(FilePath);
}

internal partial class PublicKeyManager : IPublicKeyManager
{
    Task<bool> IPublicKeyManager.ExistAsync() => Task.FromResult(true);

    async Task<PublicKey?> IPublicKeyManager.LoadAsync()
    {
        if (!FileExists())
            return null;
        var publicKeyContent = await fs.File.ReadAllTextAsync(FilePath);
        var publicKey = PublicKey.FromBase64(publicKeyContent);
        return publicKey;
    }

    async Task IPublicKeyManager.SaveAsync(PublicKey publicKey)
    {
        var publicKeyContent = publicKey.ToBase64();
        await fs.File.WriteAllTextAsync(FilePath, publicKeyContent);
    }
}
