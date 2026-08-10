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
        var content1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        var file1Path = Fs.AddFilePath("fileA.secrets.json", content1);

        // adding a file in a subdirectory
        var content2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        var file2Path = Fs.AddFilePath("subdir1/subdir2/fileB.secrets.json", content2);

        await App.RunAsync(["init"]);

        //

        await App.RunAsync(["encrypt"]);

        //

        var jsonTester1 = JsonTester.FromFile(file1Path);
        jsonTester1.GetJsonValue<string>("$.field1").Should().Be("value 1");
        jsonTester1.GetJsonValue<string>("$.field2_secret").Should().NotBe("secret value 2");

        var jsonTester2 = JsonTester.FromFile(file2Path);
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
