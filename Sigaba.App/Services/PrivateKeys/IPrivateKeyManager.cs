using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyManager
{
    Task<PrivateKey?> LoadAsync();
    Task SaveAsync(PrivateKey privateKey);
}