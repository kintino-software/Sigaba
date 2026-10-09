using Sigaba.Primitives.FileSystem;
using Xunit.Abstractions;

namespace Sigaba.Cli.IntegrationTest.Cases;

public sealed class EncryptTest : BaseTest
{
    private readonly string cwd;
    private readonly ITestOutputHelper testOutput;

    public EncryptTest(ITestOutputHelper testOutput)
    {
        var data = InitializeAppAsync().GetAwaiter().GetResult();
        cwd = data.Cwd;
        this.testOutput = testOutput;
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
            new FilePath(cwd, "fileA.secrets.json"));

        var file2Path = Fs.AddMockFilePath("""
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """,
            new FilePath(cwd, "subdir1", "subdir2", "fileB.secrets.json"));

        var file3Path = Fs.AddMockFilePath("""
            KEY=value
            KEY_SECRET=secret value
            """,
            new FilePath(cwd, "subdir1", "subdir2", ".env"));

        var file4Path = Fs.AddMockFilePath("""
            KEY=value
            KEY_SECRET=secret value
            """,
            new FilePath(cwd, ".env"));

        //

        //

        var result = await App.RunAsync(["encrypt"]);
        testOutput.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(0);

        App.Console.ShouldHaveOutputThatMatches("""
            ^4 file\(s\) affected:$
            ^\s\s.*fileA\.secrets\.json$
            ^\s\s.*\.env$
            ^\s\s.*fileB\.secrets\.json$
            ^\s\s.*\.env$
            """);

        var jsonTester1 = JsonTester.FromFile(Fs, file1Path);
        jsonTester1.GetJsonValue<string>("$.field1").Should().Be("value 1");
        jsonTester1.GetJsonValue<string>("$.field2_secret").Should().NotBe("secret value 2");

        var jsonTester2 = JsonTester.FromFile(Fs, file2Path);
        jsonTester2.GetJsonValue<string>("$.field3").Should().Be("value 3");
        jsonTester2.GetJsonValue<string>("$.field4_secret").Should().NotBe("secret value 4");

        Fs.GetFile(file3Path).TextContents.Should().NotContain("secret value");
        Fs.GetFile(file4Path).TextContents.Should().NotContain("secret value");
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
        testOutput.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(-1);
        App.Console.ShouldHaveOutputThatMatches("""
            Error: The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters\.
            """);
    }
}
