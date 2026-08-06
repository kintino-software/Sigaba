using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal partial class PrivateKeyFileRepository(IFileSystem fs) : IPrivateKeyRepository
{
    private string Cwd => fs.Directory.GetCurrentDirectory();
    private string FilePath => fs.Path.Combine(Cwd, Constants.PrivateKeyFileName);

    private bool FileExists() => fs.File.Exists(FilePath);
}

internal partial class PrivateKeyFileRepository : IPrivateKeyRepository
{
    Task<bool> IPrivateKeyRepository.ExistAsync() => Task.FromResult(FileExists());

    async Task<PrivateKey?> IPrivateKeyRepository.LoadAsync()
    {
        if (!FileExists())
            return null;
        var privateKeyContent = await fs.File.ReadAllTextAsync(FilePath);
        var privateKey = PrivateKey.FromBase64(privateKeyContent);
        return privateKey;
    }

    async Task IPrivateKeyRepository.SaveAsync(PrivateKey privateKey)
    {
        var privateKeyContent = privateKey.ToBase64();
        await fs.File.WriteAllTextAsync(FilePath, privateKeyContent);
    }



}
