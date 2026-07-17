using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services;

public interface IFacade
{
    (PublicKey, PrivateKey, EncryptedKey) CreateContextKeys();
    PlainKey DecryptKeyFromContext(IContext context);
}
