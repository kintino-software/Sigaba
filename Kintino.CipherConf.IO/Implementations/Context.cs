using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

internal class Context : IContext
{
    // IContext implementation

    public IFieldFilter FieldFilter { get => FieldFilterImpl; }
    public IFileFilter FileFilter { get => FileFilterImpl; }
    public required EncryptedKey Key { get; init; }
    public required PrivateKey PrivateKey { get; init; }
    public required PublicKey PublicKey { get; init; }

    // 

    public required FieldFilter FieldFilterImpl { get; init; }
    public required FileFilter FileFilterImpl { get; init; }
}

