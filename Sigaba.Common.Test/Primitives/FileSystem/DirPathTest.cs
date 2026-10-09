namespace Sigaba.Primitives.FileSystem;

public sealed class DirPathTest
{
    [Fact]
    public void Should_construct_from_segments()
    {
        var dir = new DirPath("a", "b", "c");
        dir.Value.Should().Be(Path.Combine("a", "b", "c"));
    }

    [Fact]
    public void Should_implicitly_convert_to_string()
    {
        var dir = new DirPath("a", "b", "c");
        string value = dir;
        value.Should().Be(Path.Combine("a", "b", "c"));
    }

    [Fact]
    public void Should_implicitly_convert_from_string()
    {
        DirPath dir = Path.Combine("a", "b", "c");
        dir.Value.Should().Be(Path.Combine("a", "b", "c"));
    }

    [Fact]
    public void Should_get_parent()
    {
        var dirPath = new DirPath("a", "b", "c");

        var b = dirPath.Parent();
        b.Should().NotBeNull();
        b.Value.Should().Be(Path.Combine("a", "b"));

        var a = b.Parent();
        a.Should().NotBeNull();
        a.Value.Should().Be("a");

        var root = a.Parent();
        root.Value.Should().BeEmpty();

        var nullParent = root.Parent();
        nullParent.Should().BeNull();
    }
}

