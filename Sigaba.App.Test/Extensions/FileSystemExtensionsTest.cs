using Sigaba.App.Services.SigabaFiles;

namespace Sigaba.App.Extensions;

public class FileSystemExtensionsTest : BaseTest
{
    // CreateFolderIfNotExists

    [Theory]
    [InlineData("a/b/c")]
    [InlineData("a")]
    [InlineData("a/b/c/d/e/f/g/h/i")]
    public void Should_create_folder_if_not_exists(string folderPath)
    {
        Fs.CreateFolderIfNotExists(folderPath.AsPath());

        Fs.Directory.Exists(folderPath.AsPath()).Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void Should_throw_when_creating_folder_with_null_or_whitespace_as_path(string folderPath)
    {
        var act = () => Fs.CreateFolderIfNotExists(folderPath.AsPath());

        act.Should().Throw<ArgumentException>().WithMessage("Folder path cannot be null or whitespace.*");
    }

    // GetNearestFileWithNameGoingUp

    [Theory]
    [InlineData("a/b/c/d", "file.txt", "a/b/file.txt")] // file is up two levels
    [InlineData("a/b", "file.txt", "a/b/file.txt")] // file is in the same level
    [InlineData("a/b/c/d/e/f/g/h/i", "file.txt", "a/b/file.txt")] // file is up many levels
    public void Should_return_nearest_file_path_going_up_the_directory_hierarchy(string startDir, string fileName, string expectedFilePath)
    {
        Fs.AddEmptyFile(expectedFilePath.AsPath());
        Fs.AddDirectory(startDir.AsPath());

        var result = Fs.GetNearestFileWithNameGoingUp(startDir.AsPath(), fileName);

        result.Should().Be(expectedFilePath.AsPath());
    }

    [Fact]
    public void Should_return_null_when_file_not_found_going_up_the_directory_hierarchy()
    {
        var startDir = "a/b/".AsPath();
        var fileName = "file.txt";
        Fs.AddEmptyFile("a/b/c/file.txt".AsPath()); // file is not up in the hierarchy, but in a subdirectory
        Fs.AddDirectory(startDir);

        var result = Fs.GetNearestFileWithNameGoingUp(startDir, fileName);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("a", null)]
    [InlineData(null, "file.txt")]
    public void Should_throw_when_getting_nearest_file_and_input_is_null_or_empty(string startDir, string fileName)
    {
        var act = () => Fs.GetNearestFileWithNameGoingUp(startDir, fileName);
        act.Should().Throw<ArgumentException>();
    }
}

