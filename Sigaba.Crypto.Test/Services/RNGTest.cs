namespace Sigaba.Crypto.Services;

public class RNGTest
{
    [Fact]
    public void Should_throw_when_size_is_less_than_1()
    {
        var action = () => RNG.GetBytes(0);
        action.Should().ThrowExactly<ArgumentException>();
    }
}

