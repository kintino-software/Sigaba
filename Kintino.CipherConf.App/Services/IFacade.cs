using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Services;

public interface IFacade
{
    (PublicKey, PrivateKey, CryptoKey) CreateContextKeys();
    PlainKey DecryptKeyFromContext(Context context);
}
