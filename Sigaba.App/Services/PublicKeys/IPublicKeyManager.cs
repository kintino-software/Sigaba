using Sigaba.Primitives;

namespace Sigaba.App.Services.PublicKeys;

internal interface IPublicKeyManager
{
    Task<bool> ExistAsync();
    Task<PublicKey?> LoadAsync();
    Task SaveAsync(PublicKey publicKey);
}
