using Sigaba.Primitives;

namespace Sigaba.App.Services.PublicKeys;

internal interface IPublicKeyRepository
{
    Task<bool> ExistAsync();
    Task<PublicKey?> LoadAsync();
    Task SaveAsync(PublicKey publicKey);
}
