using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO;

public interface IContextFactory
{
    IContext CreateDefault(PublicKey publicKey, PrivateKey privateKey);
}
