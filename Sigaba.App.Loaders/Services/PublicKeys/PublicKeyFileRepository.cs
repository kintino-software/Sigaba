using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PublicKeys;

internal partial class PublicKeyFileRepository(IFileSystem fs) : IPublicKeyRepository
{
    private string Cwd => fs.Directory.GetCurrentDirectory();
    private string FilePath => fs.Path.Combine(Cwd, Constants.PublicKeyFileName);
    private bool FileExists() => fs.File.Exists(FilePath);
}

internal partial class PublicKeyFileRepository : IPublicKeyRepository
{
    Task<bool> IPublicKeyRepository.ExistAsync() => Task.FromResult(true);

    async Task<PublicKey?> IPublicKeyRepository.LoadAsync()
    {
        if (!FileExists())
            return null;
        var publicKeyContent = await fs.File.ReadAllTextAsync(FilePath);
        var publicKey = PublicKey.FromBase64(publicKeyContent);
        return publicKey;
    }

    async Task IPublicKeyRepository.SaveAsync(PublicKey publicKey)
    {
        var publicKeyContent = publicKey.ToBase64();
        await fs.File.WriteAllTextAsync(FilePath, publicKeyContent);
    }
}
