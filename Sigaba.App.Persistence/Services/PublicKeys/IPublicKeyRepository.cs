using Sigaba.Primitives;

namespace Sigaba.App.Services.PublicKeys;

internal interface IPublicKeyRepository
{
    Task<PublicKey?> LoadAsync(string filePath);
    Task SaveAsync(PublicKey publicKey, string filePath);
}
