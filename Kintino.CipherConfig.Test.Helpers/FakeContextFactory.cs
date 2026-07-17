using Kintino.CipherConf.IO;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeContextFactory : IContextFactory
{
    private readonly FakeContext context = new();

    public IContext CreateDefault(PublicKey publicKey, PrivateKey privateKey, EncryptedKey encryptedKey)
    {
        return context;
    }

    public FakeContextFactory Returns(Action<FakeContext> configure)
    {
        configure(context);
        return this;
    }
}
