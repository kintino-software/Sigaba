using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Primitives;

public class DirPathTest
{
    private readonly MockFileSystem fs = new();

    // instantiation

    [Theory]
    [InlineData("a")]
    [InlineData("a/b")]
    [InlineData("a\\b")]
    [InlineData("a\\b/c\\d/e")]
    [InlineData("a|b")] // variadic
    [InlineData("a|b|c|d")] // variadic
    public void Should_instantiate(string path)
    {
        var parts = path.Split('/');
        var obj = new DirPath(fs, parts);

        obj.Should().NotBeNull();
        obj.Fs.Should().Be(fs);
        obj.Path.Should().Be(fs.Path.Combine(parts));
    }

    [Fact]
    public void Should_sanitize_separator_chars_on_instantiation()
    {
        var obj = new DirPath(fs, "a", "b\\c/d\\e");

        obj.Path.Should().Be(fs.Path.Combine("a", "b", "c", "d", "e"));
    }

    // equality

    [Fact]
    public void Should_be_equal_if_paths_match()
    {
        var obj1 = new DirPath(fs, "a", "b", "c");
        var obj2 = new DirPath(fs, "a", "b", "c");

        obj1.Should().Be(obj2);
        (obj1 == obj2).Should().BeTrue();
        new HashSet<DirPath> { obj1, obj2 }.Should().ContainSingle();
    }

    // Exists

    [Fact]
    public void Should_know_if_it_exists_in_file_system()
    {
        fs.AddDirectory(fs.Path.Combine("a", "b", "c"));
        var existing = new DirPath(fs, "a", "b", "c");
        var nonExisting = new DirPath(fs, "a", "b", "d");

        existing.Exists.Should().BeTrue();
        nonExisting.Exists.Should().BeFalse();
    }

    // Parent

    [Fact]
    public void Should_get_parent_if_exists()
    {
        var dir = new DirPath(fs, "a", "b", "c");
        var expectedParent = new DirPath(fs, "a", "b");

        dir.Parent().Should().Be(expectedParent);
    }

    [Fact]
    public void Should_parent_should_be_null_if_no_parent_exists()
    {
        var dir = new DirPath(fs, "a");
        var parent = dir.Parent();

        parent.Should().BeNull();
    }

    // CombineAsDir

    [Theory]
    [InlineData("a", "b", "a/b")]
    [InlineData("a", "b/c/d", "a/b/c/d")]
    [InlineData("a", "b/c\\d", "a/b/c/d")]
    [InlineData("a", "b|c|d", "a/b/c/d")]
    public void Should_combine_with_path_to_create_new_instance(string initialDirPath, string partsToCombine, string expectedDirPath)
    {
        var partsToCombineArr = partsToCombine.Split('|');
        var initial = new DirPath(fs, initialDirPath);
        var expected = new DirPath(fs, expectedDirPath);

        var actual = initial.CombineAsDir(partsToCombineArr);

        actual.Should().Be(expected);
    }

    // CombineAsFile

    [Theory]
    [InlineData("a", "b.txt", "a/b.txt")]
    [InlineData("a", "b/c/d.txt", "a/b/c/d.txt")]
    [InlineData("a", "b/c\\d.txt", "a/b/c/d.txt")]
    [InlineData("a", "b|c|d.txt", "a/b/c/d.txt")]
    public void Should_combine_with_path_to_create_new_file_instance(string initialDirPath, string partsToCombine, string expectedFilePath)
    {
        var partsToCombineArr = partsToCombine.Split('|');
        var initial = new DirPath(fs, initialDirPath);
        var expected = new FilePath(fs, expectedFilePath);

        var actual = initial.CombineAsFile(partsToCombineArr);

        actual.Should().Be(expected);
    }

    // EnsureCreated

    [Fact]
    public void Should_create_folder_when_ensuring_creation()
    {
        var dir = new DirPath(fs, "a", "b", "c");

        dir.EnsureCreated();
        var action = () => dir.EnsureCreated();

        fs.Directory.Exists(dir.Path).Should().BeTrue();
        action.Should().NotThrow();
    }

    // TryGetNearestFileWithNameGoingUp

    [Fact]
    public void Should_get_first_file_with_a_name_going_up_the_folder_tree()
    {
        fs.AddDirectory(fs.Path.Combine("a", "b", "c", "d"));
        fs.AddEmptyFile(fs.Path.Combine("a", "file.txt"));
        fs.AddEmptyFile(fs.Path.Combine("a", "b", "file.txt"));
        var dir = new DirPath(fs, "a/b/c/d");

        var result = dir.TryGetNearestFileWithNameGoingUp("file.txt", out var file);

        result.Should().BeTrue();
        file.Should().Be(new FilePath(fs, "a/b/file.txt"));
        file.Should().NotBe(new FilePath(fs, "a/file.txt"));
    }

    [Fact]
    public void Should_get_null_if_first_file_with_a_name_going_up_does_not_exists()
    {
        fs.AddDirectory(fs.Path.Combine("a", "b", "c", "d"));
        var dir = new DirPath(fs, "a/b/c/d");

        var result = dir.TryGetNearestFileWithNameGoingUp("file.txt", out var file);

        result.Should().BeFalse();
        file.Should().BeNull();
    }

}

