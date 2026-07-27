using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.IO.Implementations;

public class ContextFactoryTest
{
    [Fact]
    public void Should_create_default_context()
    {
        var service = new ContextFactory();

        var actual = service.CreateDefault(new PublicKey([1, 2, 3]), new PrivateKey([1, 2, 3]));

        actual.Should().NotBeNull().And.BeOfType<Context>();
    }
}

