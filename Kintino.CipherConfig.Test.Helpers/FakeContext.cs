using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeContext : IContext
{
    public PrivateKey PrivateKey { get; set; }
    public PublicKey PublicKey { get; set; }
    public IFieldFilter FieldFilter { get; set; }
    public IFileFilter FileFilter { get; set; }
    public EncryptedKey Key { get; set; }
}
