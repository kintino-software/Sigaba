namespace Kintino.CipherConf.IO.Adaptors;

public class PrimitiveExtensionsTest
{
    [Fact]
    public void Should_convert_to_bytes()
    {
        var expected = new byte[] { 1, 2, 3, 4, 5 };
        var base64 = expected.ToBase64String();

        var actual = base64.FromBase64String();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_convert_to_base64_string()
    {
        var expected = "AQIDBAU=";
        var bytes = Convert.FromBase64String(expected);

        var actual = bytes.ToBase64String();

        actual.Should().BeEquivalentTo(expected);
    }
}

