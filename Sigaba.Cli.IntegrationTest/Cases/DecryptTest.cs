using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.Cases;

public class DecryptTest : BaseTest
{
    private readonly string cwd;

    public DecryptTest()
    {
        cwd = CreateAndSetCwd("a", "b");
    }

    [Fact]
    public async Task Should_decript_all_files_in_directory_tree()
    {
        var file1Path = Fs.Path.Combine(cwd, "fileA_secrets.json");
        var file2Path = Fs.Path.Combine(cwd, "subdir", "fileB_secrets.json");
        var originalContent1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        var originalContent2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;

        Fs.AddFile(file1Path, new MockFileData(originalContent1));
        Fs.AddFile(file2Path, new MockFileData(originalContent2));

        var app = CreateApp();
        await app.RunAsync("init");
        await app.RunAsync("encrypt");

        //

        await app.RunAsync("decrypt");

        //

        Fs.GetFile(file1Path).TextContents.Should().Be(originalContent1);
        Fs.GetFile(file2Path).TextContents.Should().Be(originalContent2);
    }

    [Fact]
    public async Task Should_not_decript_without_private_key()
    {
        var file1Path = Fs.Path.Combine(cwd, "file_secrets.json");
        var originalContent1 = """
            {
                "field_secret": "secret value",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(originalContent1));

        var app = CreateApp();
        await app.RunAsync("init");
        Fs.RemoveFile("private.key"); // remove private key to simulate missing key
        await app.RunAsync("encrypt");

        //

        var action = () => app.RunAsync("decrypt");

        //

        await action.Should().ThrowAsync<Exception>();
    }
}
