using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Documents.TestHelpers;

public class BaseTest
{
    protected ISymmetricCipher SymmetricCipherMock { get; } = Substitute.For<ISymmetricCipher>();
    protected IAsymmetricCipher AsymmetricCipherMock { get; } = Substitute.For<IAsymmetricCipher>();
    protected INonceGenerator NonceGeneratorMock { get; } = Substitute.For<INonceGenerator>();
    protected MockFileSystem Fs { get; } = new();

    public BaseTest()
    {
        // define this default behaviour because we need to encrypt and decrypt back to original value in the tests,
        SymmetricCipherMock.Encrypt(default, default, default).ReturnsForAnyArgs(FlipPlain);
        SymmetricCipherMock.Decrypt(default, default, default).ReturnsForAnyArgs(FlipCrypto);
        AsymmetricCipherMock.Encrypt(default, default).ReturnsForAnyArgs(FlipPlain);
        AsymmetricCipherMock.Decrypt(default, default).ReturnsForAnyArgs(FlipCrypto);

        NonceGeneratorMock.NewNonce().Returns(Nonce.FakeNonce());
    }

    private static CryptoBytes FlipPlain(NSubstitute.Core.CallInfo call) => new(FlipBytes(call.Arg<PlainBytes>().Bytes));
    private static PlainBytes FlipCrypto(NSubstitute.Core.CallInfo call) => new(FlipBytes(call.Arg<CryptoBytes>().Bytes));
    private static byte[] FlipBytes(byte[] bytes) => [.. bytes.Reverse()];

}
