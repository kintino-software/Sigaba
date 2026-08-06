using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyRepository
{
    Task<bool> ExistAsync();
    Task<PrivateKey?> LoadAsync();
    Task SaveAsync(PrivateKey privateKey);
}