using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

internal class ContextFactory : IContextFactory
{
    public IContext CreateDefault(PublicKey publicKey, PrivateKey privateKey)
    {
        return new Context()
        {
            FieldFilterImpl = new FieldFilter(@"_secret$"),
            FileFilterImpl = new FileFilter(includePattern: @"appsettings\.?.*\.json", excludePattern: null),
            PrivateKey = privateKey,
            PublicKey = publicKey
        };
    }
}
