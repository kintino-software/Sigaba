using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Services;

namespace Kintino.CipherConf.App.TestHelpers;

public abstract class BaseTest
{
    protected IFacade Facade { get; } = Substitute.For<IFacade>();
    protected IAsymmetricCipher AsymmetricCipher { get; } = Substitute.For<IAsymmetricCipher>();
    protected IContextRepository ContextRepository { get; } = Substitute.For<IContextRepository>();
    protected IFileCipher FileCipher { get; } = Substitute.For<IFileCipher>();
    protected IFileOperations FileOperations { get; } = Substitute.For<IFileOperations>();
    protected INonceGenerator NonceGenerator { get; } = Substitute.For<INonceGenerator>();
    protected IRandomKeyGenerator RandomKeyGenerator { get; } = Substitute.For<IRandomKeyGenerator>();
    protected ISymmetricCipher SymmetricCipher { get; } = Substitute.For<ISymmetricCipher>();
    protected ITextEditor TextEditor { get; } = Substitute.For<ITextEditor>();
}
