namespace Sigaba.Cli.Cases;

public sealed class EncryptTest : BaseTest
{
    private readonly string cwd;

    public EncryptTest()
    {
        var data = InitializeAppAsync().GetAwaiter().GetResult();
        cwd = data.Cwd;
    }

    // tests

    [Fact]
    public async Task Should_encrypt_all_files_in_directory_tree()
    {
        var file1Path = Fs.AddMockFilePath("""
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """,
            cwd, "fileA.secrets.json");

        var file2Path = Fs.AddMockFilePath("""
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """,
            cwd, "subdir1", "subdir2", "fileB.secrets.json");

        //

        var result = await App.RunAsync(["encrypt"]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(0);

        var jsonTester1 = JsonTester.FromFile(file1Path);
        jsonTester1.GetJsonValue<string>("$.field1").Should().Be("value 1");
        jsonTester1.GetJsonValue<string>("$.field2_secret").Should().NotBe("secret value 2");

        var jsonTester2 = JsonTester.FromFile(file2Path);
        jsonTester2.GetJsonValue<string>("$.field3").Should().Be("value 3");
        jsonTester2.GetJsonValue<string>("$.field4_secret").Should().NotBe("secret value 4");

        App.Console.ShouldHaveOutputThatMatches("""
            2 file\(s\) encrypted:
              .*fileA\.secrets\.json
              .*fileB\.secrets\.json
            """);
    }

    [Fact]
    public async Task Should_not_encrypt_with_invalid_public_key()
    {
        var file1Path = Fs.AddMockFilePath("""
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """,
            "file.secrets.json");
        // messing with the key so that it is invalid
        JsonTester.EditJsonFileInPlace<string>(Fs, "sigaba.json", "$.meta.publicKey", value => "a" + value);

        //

        var result = await App.RunAsync(["encrypt"]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(-1);
        App.Console.ShouldHaveOutputThatMatches("""
            Error: The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters\.
            """);
    }
}
