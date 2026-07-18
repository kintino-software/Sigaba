using Kintino.CipherConf.IO.Models;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

internal class ContextFactory : IContextFactory
{
    public IContext CreateDefault(PublicKey publicKey, PrivateKey privateKey, EncryptedKey encryptedKey)
    {
        return new ConcreteContext()
        {
            FieldFilter = new RegexFilter(@"_secret$"),
            FileFilter = new RegexFilter(@"appsettings\.?.*\.json"),
            PrivateKey = privateKey,
            PublicKey = publicKey,
            Key = encryptedKey
        };
    }
}
