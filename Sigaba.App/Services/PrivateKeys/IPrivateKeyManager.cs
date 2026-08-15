using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyManager
{
    Task SaveAsync(PrivateKey privateKey, FilePath path, string password);
    Task<PrivateKey?> LoadAsync(FilePath path, string password);
}