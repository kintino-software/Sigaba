using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Primitives;

internal record ConcreteContext : IContext, ISerializable
{
    public required PrivateKey PrivateKey { get; init; }
    public required PublicKey PublicKey { get; init; }
    public required IFieldFilter FieldFilter { get; init; }
    public required IFileFilter FileFilter { get; init; }
    public required EncryptedKey Key { get; init; }
}