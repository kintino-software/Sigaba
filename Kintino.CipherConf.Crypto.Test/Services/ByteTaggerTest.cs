using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto.Services;

public class ByteTaggerTest
{
    private readonly byte[] bytes = [8, 8, 8];
    private readonly PlainData data;

    public ByteTaggerTest()
    {
        data = new PlainData(bytes);
    }

    // Tag

    [Fact]
    public void Should_tag_byte_array_in_the_last_position()
    {
        data.Bytes[0].Should().NotBe(16);
        var tagged = ByteTagger.Tag(data, 16);

        tagged.Bytes[0].Should().Be(16);
    }

    [Fact]
    public void Should_get_untagged_data()
    {
        var tagged = ByteTagger.Tag(data, 16);
        tagged.Bytes[0].Should().Be(16);

        var untagged = ByteTagger.Untag(tagged, out var tag);
        untagged.Should().BeEquivalentTo(data, opt => opt.WithStrictOrdering());
        tag.Should().Be(16);
    }

    [Fact]
    public void Should_throw_when_untagging_invalid_data()
    {
        var data = new PlainData([]);

        Action act = () => ByteTagger.Untag(data, out var tag);

        act.Should().Throw<InvalidOperationException>();
    }
}

