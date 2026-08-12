using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyManager
{
    Task<PrivateKey> LoadAsync(Guid projectId, string password, DirPath? customLocation);
    Task SaveAsync(Guid projectId, PrivateKey privateKey, string password, DirPath? customLocation);
}