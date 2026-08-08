using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.Cases;

public class EncryptionTest : BaseTest
{
    private readonly string cwd;

    public EncryptionTest()
    {
        cwd = CreateAndSetCwd("a/b".AsPath());
    }

    [Fact]
    public async Task Should_encrypt_all_files_in_directory_tree()
    {
        // adding file in the root
        var file1Path = $"{cwd}/fileA.secrets.json".AsPath();
        var content1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(content1));

        // adding a file in a subdirectory
        var file2Path = $"{cwd}/subdir1/subdir2/fileB.secrets.json".AsPath();
        var content2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        Fs.AddFile(file2Path, new MockFileData(content2));

        var app = CreateApp();
        await app.RunAsync("init");

        //

        await app.RunAsync("encrypt");

        //

        var file1content = await Fs.File.ReadAllTextAsync(file1Path);
        file1content.GetJsonValue<string>("$.field1").Should().Be("value 1");
        file1content.GetJsonValue<string>("$.field2_secret").Should().NotBe("secret value 2");

        var file2Content = await Fs.File.ReadAllTextAsync(file2Path);
        file2Content.GetJsonValue<string>("$.field3").Should().Be("value 3");
        file2Content.GetJsonValue<string>("$.field4_secret").Should().NotBe("secret value 4");
    }

    [Fact]
    public async Task Should_not_encrypt_without_valid_public_key()
    {
        var file1Path = $"{cwd}/file_secrets.json".AsPath();
        var content1 = """
            {
                "field_secret": "secret value",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(content1));

        var app = CreateApp();
        await app.RunAsync("init");
        Fs.EditJsonFile<string>("sigaba.json", "$.publicKey", value => (value + "x")); // messing with the key so that it is invalid

        //

        var action = () => app.RunAsync("encrypt");

        await action.Should().ThrowAsync<Exception>();
    }
}
