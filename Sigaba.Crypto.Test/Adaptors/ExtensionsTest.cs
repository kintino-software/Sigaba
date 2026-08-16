namespace Sigaba.Crypto.Adaptors;

public class ExtensionsTest
{
    private readonly byte[] bytes = [8, 8, 8];

    // Tag

    [Fact]
    public void Should_tag_byte_array_in_the_last_position()
    {

        var tagged = bytes.Tag(16);
        tagged[0].Should().Be(16);
    }

    // Untag

    [Fact]
    public void Should_get_untagged_data()
    {
        var tagged = bytes.Tag(16);
        tagged[0].Should().Be(16);

        var untagged = tagged.Untag(out var tag);
        untagged.Should().BeEquivalentTo(bytes, opt => opt.WithStrictOrdering());
        tag.Should().Be(16);
    }

    [Fact]
    public void Should_throw_when_untagging_invalid_data()
    {
        var data = Array.Empty<byte>();

        Action act = () => data.Untag(out var tag);

        act.Should().Throw<InvalidOperationException>();
    }
}

