using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyManager
{
    Task<PrivateKey> LoadAsync(Guid projectId, string password);
    Task SaveAsync(Guid projectId, PrivateKey privateKey, string password);
}