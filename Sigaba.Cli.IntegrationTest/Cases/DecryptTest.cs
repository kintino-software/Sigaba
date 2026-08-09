using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.Cases;

public class DecryptTest : BaseTest
{
    private readonly string cwd;

    public DecryptTest()
    {
        cwd = Fs.SafeSetCwd("testdir");
    }

    [Fact]
    public async Task Should_decrypt_all_files_in_directory_tree()
    {
        var file1Path = Fs.SafePath(cwd, "fileA_secrets.json");
        var originalContent1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(originalContent1));

        var file2Path = Fs.SafePath(cwd, "subdir", "fileB_secrets.json");
        var originalContent2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        Fs.AddFile(file2Path, new MockFileData(originalContent2));

        await App.RunAsync(["init"]);
        await App.RunAsync(["encrypt"]);

        //

        await App.RunAsync(["decrypt"]);

        //

        Fs.GetFile(file1Path).TextContents.Should().Be(originalContent1);
        Fs.GetFile(file2Path).TextContents.Should().Be(originalContent2);
    }

    [Fact]
    public async Task Should_not_decript_without_private_key()
    {
        var filePath = Fs.SafePath(cwd, "file_secrets.json");
        var content = """
            {
                "field_secret": "secret value",
            }
            """;
        Fs.AddFile(filePath, new MockFileData(content));
        await App.RunAsync(["init"]);

        //

        Fs.RemoveFile(Fs.AllFiles.First(f => f.EndsWith("private.key"))); // remove private key to simulate missing key
        var action = () => App.RunAsync(["decrypt"]);

        //

        await action.Should().ThrowAsync<Exception>();
    }
}
