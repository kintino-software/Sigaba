using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyRepository
{
    Task<PrivateKey?> LoadAsync(string filePath);
    Task SaveAsync(PrivateKey privateKey, string filePath);
}