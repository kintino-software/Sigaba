using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.Cases;

public class EncryptionTest : BaseTest
{
    private readonly string cwd;

    public EncryptionTest()
    {
        cwd = Fs.SafeSetCwd("testdir");
    }

    [Fact]
    public async Task Should_encrypt_all_files_in_directory_tree()
    {
        // adding file in the root
        var file1Path = Fs.SafePath(cwd, "fileA.secrets.json");
        var content1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(content1));

        // adding a file in a subdirectory
        var file2Path = Fs.SafePath(cwd, "subdir1/subdir2/fileB.secrets.json");
        var content2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        Fs.AddFile(file2Path, new MockFileData(content2));

        await App.RunAsync(["init"]);

        //

        await App.RunAsync(["encrypt"]);

        //

        var jsonTester1 = JsonTester.FromFile(Fs, file1Path);
        jsonTester1.GetJsonValue<string>("$.field1").Should().Be("value 1");
        jsonTester1.GetJsonValue<string>("$.field2_secret").Should().NotBe("secret value 2");

        var jsonTester2 = JsonTester.FromFile(Fs, file2Path);
        jsonTester2.GetJsonValue<string>("$.field3").Should().Be("value 3");
        jsonTester2.GetJsonValue<string>("$.field4_secret").Should().NotBe("secret value 4");
    }

    [Fact]
    public async Task Should_not_encrypt_without_valid_public_key()
    {
        var file1Path = Fs.SafePath(cwd, "file_secrets.json");
        var content1 = """
            {
                "field_secret": "secret value",
            }
            """;
        Fs.AddFile(file1Path, new MockFileData(content1));

        await App.RunAsync(["init"]);
        JsonTester.EditJsonFileInPlace<string>(Fs, "sigaba.json", "$.publicKey", value => (value + "x")); // messing with the key so that it is invalid

        //

        var action = () => App.RunAsync(["encrypt"]);

        await action.Should().ThrowAsync<Exception>();
    }
}
