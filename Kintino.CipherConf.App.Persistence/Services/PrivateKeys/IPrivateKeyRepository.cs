using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.PrivateKeys;

internal interface IPrivateKeyRepository
{
    Task<PrivateKey?> LoadAsync(string filePath);
    Task SaveAsync(PrivateKey privateKey, string filePath);
}