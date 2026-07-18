using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.TestHelpers;

internal class FakeContext : IContext
{
    public PrivateKey PrivateKey { get; } = new(new([1, 2, 3]));
    public PublicKey PublicKey { get; } = new(new([4, 5, 6]));
    public IFieldFilter FieldFilter { get; } = Substitute.For<IFieldFilter>();
    public IFileFilter FileFilter { get; } = Substitute.For<IFileFilter>();
    public EncryptedKey Key { get; } = new(new([7, 8, 9]));

    public FakeContext(Func<string, bool> fieldFilter = null, Func<string, bool> fileFilter = null)
    {
        FieldFilter.Match(default)
            .ReturnsForAnyArgs(ci =>
            {
                return fieldFilter == null || fieldFilter(ci.ArgAt<string>(0));
            });

        FileFilter.Match(default)
            .ReturnsForAnyArgs(ci =>
            {
                return fileFilter == null || fileFilter(ci.ArgAt<string>(0));
            });
    }
}
