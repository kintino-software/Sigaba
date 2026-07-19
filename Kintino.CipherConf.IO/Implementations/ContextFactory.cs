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
            SerializableFieldFilter = new SerializableFieldFilter(@"_secret$"),
            SerializableFileFilter = new SerializableFileFilter(includePattern: @"appsettings\.?.*\.json", excludePattern: null),
            SerializablePrivateKey = new SerializablePrivateKey(privateKey),
            SerializablePublicKey = new SerializablePublicKey(publicKey),
            SerializableKey = new SerializableKey(encryptedKey)
        };
    }
}
