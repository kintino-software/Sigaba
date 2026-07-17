using Kintino.CipherConfig;

namespace Kintino.CipherConf.IO.Implementations;

public class FileOperationsTest : BaseTest
{
    private IFileOperations CreateService() => new FileOperations(Fs);

    // CopyWithOverwrite

    [Fact]
    public async Task Should_copy_file_to_new_location()
    {
        this.Fs.AddFile("original.txt", new MockFileData("Original content"));
        var service = CreateService();

        await service.CopyWithOverwrite("original.txt", "copy.txt");

        this.Fs.FileExists("copy.txt").Should().BeTrue();
        this.Fs.GetFile("copy.txt").TextContents.Should().Be("Original content");
    }

    [Fact]
    public async Task Should_copy_and_overwrite_existing_file()
    {
        this.Fs.AddFile("original.txt", new MockFileData("Original content"));
        this.Fs.AddFile("existing.txt", new MockFileData("Existing content"));
        var service = CreateService();

        await service.CopyWithOverwrite("original.txt", "existing.txt");

        this.Fs.GetFile("existing.txt").TextContents.Should().Be("Original content");
    }

    // GetFilesFromDirectory

    [Fact]
    public async Task Should_return_files_matching_pattern()
    {
        this.Fs.AddFile("file1.txt", new MockFileData("Content 1"));
        this.Fs.AddFile("file2.log", new MockFileData("Content 2"));
        this.Fs.AddFile("file3.txt", new MockFileData("Content 3"));
        var filter = new FakeFileFilter().SetMatchFunc(f => f.EndsWith(".txt"));
        var service = CreateService();

        var files = await service.GetFilesFromDirectory(".", filter);

        files.Should().Contain([
            Fs.Path.Combine(RootPath, "file1.txt"),
            Fs.Path.Combine(RootPath, "file3.txt")
        ]);
        files.Should().NotContain(Fs.Path.Combine(RootPath, "file2.log"));
    }

    // WithTempFile

    [Fact]
    public async Task Should_create_temp_file_and_invoke_operations()
    {
        this.Fs.AddFile("original.txt", new MockFileData("Original content"));
        var service = CreateService();
        bool editingOperationInvoked = false;
        bool beforeDeleteOperationInvoked = false;
        string editOperationFilePath = null;
        string beforeDeleteOperationFilePath = null;

        //

        TempFileEditOperation editOperation = (filePath) =>
        {
            Fs.File.Exists(filePath).Should().BeTrue();
            editOperationFilePath = filePath;
            editingOperationInvoked = true;
            return ValueTask.CompletedTask;
        };

        TempFileBeforeDeleteOperation beforeDeleteOperation = (filePath) =>
        {
            Fs.File.Exists(filePath).Should().BeTrue();
            beforeDeleteOperationFilePath = filePath;
            beforeDeleteOperationInvoked = true;
            return ValueTask.CompletedTask;
        };

        await service.WithTempFile("original.txt", editOperation, beforeDeleteOperation);

        //

        editingOperationInvoked.Should().BeTrue();
        beforeDeleteOperationInvoked.Should().BeTrue();
        editOperationFilePath.Should().NotBeNull();
        beforeDeleteOperationFilePath.Should().NotBeNull();
        editOperationFilePath.Should().Be(beforeDeleteOperationFilePath);
        Fs.FileExists(editOperationFilePath).Should().BeFalse(); // Temp file should be deleted after operations
    }
}

