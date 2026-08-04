using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.Cases;

public class EncryptionTest : BaseTest
{
    private readonly string cwd;

    public EncryptionTest()
    {
        cwd = CreateAndSetCwd("a", "b");
    }

    [Fact]
    public async Task Should_encrypt_all_files_in_directory_tree()
    {
        var file1Path = Fs.Path.Combine(cwd, "fileA_secrets.json");
        var file2Path = Fs.Path.Combine(cwd, "subdir", "fileB_secrets.json");
        var content1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        var content2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(content1));
        Fs.AddFile(file2Path, new MockFileData(content2));

        var app = CreateApp();
        await app.RunAsync("init");

        //

        await app.RunAsync("encrypt");

        //

        Fs.InspectJson(file1Path)
            .ShouldHavePropertyWithValue("field1", "\"value 1\"")
            .ShouldHavePropertyWithValueThatIsNot("field2_secret", "\"secret value 2\"");
        Fs.InspectJson(file2Path)
            .ShouldHavePropertyWithValue("field3", "\"value 3\"")
            .ShouldHavePropertyWithValueThatIsNot("field4_secret", "\"secret value 4\"");
    }

    [Fact]
    public async Task Should_not_encrypt_without_public_key()
    {
        var file1Path = Fs.Path.Combine(cwd, "file_secrets.json");
        var content1 = """
            {
                "field_secret": "secret value",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(content1));

        var app = CreateApp();
        await app.RunAsync("init");
        Fs.RemoveFile("public.key"); // remove public key to simulate missing key

        //

        var action = () => app.RunAsync("encrypt");

        await action.Should().ThrowAsync<Exception>();
    }
}
