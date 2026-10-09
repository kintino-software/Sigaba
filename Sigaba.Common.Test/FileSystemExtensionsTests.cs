using Sigaba.Primitives.FileSystem;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba;

public class FileSystemExtensionsTest
{
    private readonly MockFileSystem fs = new();

    private string Cwd() => fs.Directory.GetCurrentDirectory();

    [Fact]
    public void Should_ensure_directory_is_created()
    {
        var dir = new DirPath(Cwd(), "/test/directory");
        fs.Directory.Exists(dir).Should().BeFalse();

        fs.EnsureDirectoryExists(dir);

        fs.Directory.Exists(dir).Should().BeTrue();
    }

    [Fact]
    public async Task Should_write_file_safely()
    {
        var dir = new DirPath(Cwd(), "/test/directory");
        var file = new FilePath(dir, "file.txt");
        var content = "Hello, world!";
        fs.Directory.Exists(dir).Should().BeFalse();

        await fs.SafeWriteAllTextAsync(file, content, allowOverwrite: false);

        fs.File.Exists(file).Should().BeTrue();

        fs.File.ReadAllText(file).Should().Be(content);
    }

    [Fact]
    public async Task Should_overwrite_file_when_allowed()
    {
        var file = new FilePath(Cwd(), "file.txt");

        await fs.SafeWriteAllTextAsync(file, "old content", allowOverwrite: true);
        await fs.SafeWriteAllTextAsync(file, "new content", allowOverwrite: true);

        fs.File.ReadAllText(file).Should().Be("new content");
    }

    [Fact]
    public async Task Should_write_prevent_overwrite()
    {
        var file = new FilePath(Cwd(), "file.txt");
        var content = "Hello, world!";

        await fs.SafeWriteAllTextAsync(file, content, allowOverwrite: true);

        var action = async () => await fs.SafeWriteAllTextAsync(file, content, allowOverwrite: false);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage($"File '{file}' already exists and overwriting is not allowed.");
    }

}

