using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Primitives.FileSystem;

public class FilePathTest
{
    private readonly MockFileSystem fs = new();

    // Exists

    [Fact]
    public void Should_know_if_it_exists_in_file_system()
    {
        fs.AddEmptyFile(fs.Path.Combine("a", "b", "c.txt"));
        var existing = new FilePath(fs, "a", "b", "c.txt");
        var nonExisting = new FilePath(fs, "a", "b", "d.txt");

        existing.Exists.Should().BeTrue();
        nonExisting.Exists.Should().BeFalse();
    }

    // ExtensionWithDot

    [Fact]
    public void Should_know_its_extension_with_dot()
    {
        var obj = new FilePath(fs, "a", "b", "c.txt");

        obj.ExtensionWithDot.Should().Be(".txt");
    }

    // Parent

    [Fact]
    public void Should_get_its_parent_directory_if_exists()
    {
        var file = new FilePath(fs, "a", "b", "c.txt");
        var expectedParent = new DirPath(fs, "a", "b");

        file.Parent().Should().Be(expectedParent);
    }

    // WriteAsync

    [Fact]
    public async Task Should_write_content_async()
    {
        var file = new FilePath(fs, "a.txt");

        await file.WriteAsync("content", overwrite: false, createFolders: true);

        fs.GetFile(file.Path).TextContents.Should().Be("content");
    }

    [Fact]
    public async Task Should_throw_when_overwritting_an_existing_file_when_overwrite_is_false()
    {
        var file = new FilePath(fs, "a", "b", "c.txt");
        await file.WriteAsync("content", overwrite: false, createFolders: true);

        var act = async () => await file.WriteAsync("new content", overwrite: false, createFolders: true);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("File already exists and overwrite is set to false.");
    }

    [Fact]
    public async Task Should_throw_when_writting_to_an_inexistent_folder_when_createFolders_is_false()
    {
        var file = new FilePath(fs, "a", "b", "c.txt");

        var act = async () => await file.WriteAsync("content", overwrite: false, createFolders: false);

        await act.Should().ThrowAsync<DirectoryNotFoundException>();
    }

    [Fact]
    public async Task Should_create_folder_structure_when_writing_content_async()
    {
        var file = new FilePath(fs, "a", "b", "c", "d", "e", "f.txt");

        await file.WriteAsync("content", overwrite: false, createFolders: true);

        fs.GetFile(file.Path).TextContents.Should().Be("content");
    }

    // Write

    [Fact]
    public void Should_write_synchronously()
    {
        var file = new FilePath(fs, "a", "b", "c", "d", "e", "f.txt");

        file.Write("content", overwrite: false, createFolders: true);

        fs.GetFile(file.Path).TextContents.Should().Be("content");
    }

    // ReadAsync

    [Fact]
    public async Task Should_read_file_content()
    {
        var file = new FilePath(fs, "a.txt");
        await file.WriteAsync("content", overwrite: false, createFolders: true);

        var content = await file.ReadAsync();

        content.Should().Be("content");
    }

    [Fact]
    public async Task Should_throw_when_reading_a_nonexistent_file()
    {
        var file = new FilePath(fs, "a.txt");

        var act = async () => await file.ReadAsync();

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    // Read

    [Fact]
    public void Should_read_file_content_synchronously()
    {
        var file = new FilePath(fs, "a.txt");
        file.Write("content", overwrite: false, createFolders: true);

        var content = file.Read();

        content.Should().Be("content");
    }
    // AssertExists

    [Fact]
    public void Should_throw_when_asserting_existence_of_a_nonexistent_file()
    {
        var file = new FilePath(fs, "a.txt");
        var act = () => file.AssertExists();
        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void Should_not_throw_when_asserting_existence_of_an_existing_file()
    {
        var file = new FilePath(fs, "a.txt");
        file.Write("content", overwrite: false, createFolders: true);

        var act = () => file.AssertExists();

        act.Should().NotThrow();
    }
}

