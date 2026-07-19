using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

public class ContextFactoryTest
{
    [Fact]
    public void Should_create_default_context()
    {
        var service = new ContextFactory();

        var actual = service.CreateDefault(new PublicKey(new([1, 2, 3])), new PrivateKey(new([1, 2, 3])), new EncryptedKey(new([1, 2, 3])));

        actual.Should().NotBeNull().And.BeOfType<Context>();
    }
}

