using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyManager
{
    Task<bool> ExistAsync();
    Task<PrivateKey?> LoadAsync();
    Task SaveAsync(PrivateKey privateKey);
}