namespace Sigaba.Primitives.Crypto.Base;

public class ByteLikeTests
{
    private record DummyByteLike(byte[] Bytes) : ByteLike<DummyByteLike>(Bytes);

    // ToBase64

    [Fact]
    public void Should_convert_to_base64_string()
    {
        var obj = new DummyByteLike([1, 2, 3]);
        var actual = obj.ToBase64();
        actual.Should().Be("AQID");
    }

    [Fact]
    public void Should_not_create_url_and_file_name_unsafe_chars_when_converting_to_base64()
    {
        var obj = new DummyByteLike([251, 255, 254]);
        var actual = obj.ToBase64();
        actual.Should().Be("-__-");
    }

    // FromBase64

    [Fact]
    public void Should_convert_from_base64_string()
    {
        var base64String = "AQID";
        var actual = DummyByteLike.FromBase64(base64String);
        actual.Bytes.Should().Equal([1, 2, 3]);
    }

    // Roundtrip

    [Fact]
    public void Should_roundtrip_to_and_from_base64_string()
    {
        var original = new DummyByteLike([0, 1, 3, 4, 5, 251, 255, 254]);
        var originalBase64 = original.ToBase64();

        var actual = DummyByteLike.FromBase64(originalBase64);

        actual.Bytes.Should().BeEquivalentTo(original.Bytes);
    }

}

