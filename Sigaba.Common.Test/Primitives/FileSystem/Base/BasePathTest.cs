using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Primitives.FileSystem.Base;

public class BasePathTest
{
    private readonly MockFileSystem fs = new();

    private class DummyPath(IFileSystem fs, params string[] parts) : BasePath(fs, parts);

    // instantiation

    [Fact]
    public void Should_instantiate()
    {

        List<(string[], string)> inputExpectedList = [
            (["a.txt"],                 fs.Path.Combine("a.txt")),
            (["a", "b.txt"],            fs.Path.Combine("a", "b.txt")),
            (["a", "b", "c", "d.txt"],  fs.Path.Combine("a", "b", "c", "d.txt"))
        ];

        foreach (var (input, expected) in inputExpectedList)
        {
            var obj = new DummyPath(fs, input);

            obj.Should().NotBeNull();
            obj.Fs.Should().Be(fs);
            obj.Path.Should().Be(expected);
        }
    }

    // IsAbsolute

    [Fact]
    public void Should_have_know_if_its_absolute()
    {
        fs.Directory.SetCurrentDirectory("cwd");
        var cwd = fs.Directory.GetCurrentDirectory();

        new DummyPath(fs, cwd).IsAbsolute.Should().BeTrue();
        new DummyPath(fs, cwd, "a", "b").IsAbsolute.Should().BeTrue();
        new DummyPath(fs, "a").IsAbsolute.Should().BeFalse();
        new DummyPath(fs, "a", "b").IsAbsolute.Should().BeFalse();
    }

    // equality

    [Fact]
    public void Should_be_equal_if_paths_match()
    {
        var obj1 = new DummyPath(fs, "a", "b", "c");
        var obj2 = new DummyPath(fs, "a", "b", "c");
        var obj3 = new DummyPath(fs, "a", "b");

        obj1.Should().Be(obj2);
        obj1.Should().NotBe(obj3);
        obj1.Equals(obj2).Should().BeTrue();
        obj2.Equals(obj3).Should().BeFalse();
        (obj1 == obj2).Should().BeTrue();
        (obj1 == obj3).Should().BeFalse();
    }

    // hash code

    [Fact]
    public void Should_have_same_hash_code_if_paths_match()
    {
        var obj1 = new DummyPath(fs, "a", "b", "c");
        var obj2 = new DummyPath(fs, "a", "b", "c");
        var obj3 = new DummyPath(fs, "a", "b");

        obj1.GetHashCode().Should().Be(obj2.GetHashCode());
        obj1.GetHashCode().Should().NotBe(obj3.GetHashCode());
        new HashSet<DummyPath> { obj1, obj2 }.Should().ContainSingle();
        new HashSet<DummyPath> { obj1, obj3 }.Should().HaveCount(2);
    }

}
