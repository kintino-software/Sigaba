using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.Serializers;

internal interface IContextLoader
{
    Task CreateContextAsync(PublicKey publicKey, PrivateKey privateKey);
    Task<bool> HasContextAsync();
    Task<Context?> LoadContextAsync();
}