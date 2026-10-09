namespace Sigaba.Primitives.FileSystem;

public class FilePathTest
{
    [Fact]
    public void Should_instantiate_from_segments()
    {
        var file = new FilePath("a", "b", "c.txt");
        file.Value.Should().Be(Path.Combine("a", "b", "c.txt"));
    }

    [Fact]
    public void Should_implicitly_convert_to_string()
    {
        var file = new FilePath("a", "b", "c.txt");
        string value = file;
        value.Should().Be(Path.Combine("a", "b", "c.txt"));
    }

    [Fact]
    public void Should_implicitly_convert_from_string()
    {
        FilePath file = Path.Combine("a", "b", "c.txt");
        file.Value.Should().Be(Path.Combine("a", "b", "c.txt"));
    }
}

