using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.TestHelpers;

public abstract class BaseTest
{
    protected ISymmetricCipher SymmetricCipher { get; } = Substitute.For<ISymmetricCipher>();

    protected BaseTest()
    {
        // we add some symmetric cipher behavior so that we can rely on a round-trip encryption and decryption of values
        SymmetricCipher.Encrypt(default, default, default)
            .ReturnsForAnyArgs(ci => new EncryptedData([.. ci.ArgAt<PlainData>(1).Bytes.Reverse()]));
        SymmetricCipher.Decrypt(default, default, default)
            .ReturnsForAnyArgs(ci => new PlainData([.. ci.ArgAt<EncryptedData>(1).Bytes.Reverse()]));
    }
}
