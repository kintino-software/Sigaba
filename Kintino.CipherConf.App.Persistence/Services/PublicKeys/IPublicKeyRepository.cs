using Kintino.CipherConf.Primitives;
using Kintino.CipherConf.App.Services;

namespace Kintino.CipherConf.App.Services.PublicKeys;

internal interface IPublicKeyRepository
{
    Task<PublicKey?> LoadAsync(string filePath);
    Task SaveAsync(PublicKey publicKey, string filePath);
}
