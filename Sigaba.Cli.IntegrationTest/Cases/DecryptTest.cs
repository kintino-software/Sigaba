using Sigaba.Primitives.FileSystem;
using Xunit.Abstractions;

namespace Sigaba.Cli.IntegrationTest.Cases;

public class DecryptTest : BaseTest
{
    private readonly string cwd;
    private readonly string password;
    private readonly ITestOutputHelper testOutput;

    public DecryptTest(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
        InitializeAppAsync().GetAwaiter().GetResult().Deconstruct(out password, out cwd);
    }

    private async Task Encrypt()
    {
        var result = await CreateCommandApp().RunAsync(["encrypt"]);
        result.ExitCode.Should().Be(0);
    }

    // tests

    [Theory]
    [InlineData("decrypt", "-p")]
    [InlineData("decrypt", "--password")]
    public async Task Should_decrypt_all_files_in_directory_tree(string command, string passwordArg)
    {
        var originalContent1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        var path1 = Fs.AddMockFilePath(originalContent1, new FilePath(cwd, "file1.secrets.json"));

        var originalContent2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        var path2 = Fs.AddMockFilePath(originalContent2, new FilePath(cwd, "dir", "file2.secrets.json"));

        var originalContent3 = """
            KEY1_SECRET=secret value 1         
            KEY2=normal value 2
            """;
        var path3 = Fs.AddMockFilePath(originalContent3, new FilePath(cwd, "dir", "subdir", ".env"));

        await Encrypt();

        //

        var result = await App.RunAsync([command, passwordArg, password]);
        testOutput.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(0);
        Fs.GetFile(path1).TextContents.Should().Be(originalContent1);
        Fs.GetFile(path2).TextContents.Should().Be(originalContent2);
        Fs.GetFile(path3).TextContents.Should().Be(originalContent3);
        App.Console.ShouldHaveOutputThatMatches("""
            ^3 file\(s\) affected:$
            ^\s\s.*file1\.secrets\.json$
            ^\s\s.*file2\.secrets\.json$
            ^\s\s.*\.env$
            """);
    }

    [Theory]
    [InlineData("--private-key-path")]
    [InlineData("-k")]
    public async Task Should_decrypt_files_with_provided_private_key_path(string command)
    {
        Fs.AddMockFilePath(
            """
            {
                "field2_secret": "secret value 2",
            }
            """,
            new FilePath(cwd, "file.secrets.json"));

        var privateKeyPath = Fs.AllFiles.First(f => f.EndsWith("private.key"));
        var newLocation = new FilePath(cwd, "new-location", "private.key");
        Fs.EnsureDirectoryExists(newLocation.GetContainingDirectory());
        Fs.File.Move(privateKeyPath, newLocation);

        await Encrypt();

        //

        var result = await App.RunAsync(["encrypt", "-p", password, command, newLocation]);
        testOutput.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(0);

        App.Console.ShouldHaveOutputThatMatches("""
            ^1 file\(s\) affected:$
            ^\s\s.*file\.secrets\.json$
            """);
    }

    [Fact]
    public async Task Should_not_decript_without_private_key()
    {
        Fs.RemoveFile(Fs.AllFiles.First(f => f.EndsWith("private.key"))); // remove private key to simulate missing key

        //

        var result = await App.RunAsync(["decrypt", "-p", password]);
        testOutput.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().NotBe(0);
        App.Console.ShouldHaveOutputThatMatches(@"Error: Private key not found on any of expected locations\.");
    }

    [Fact]
    public async Task Should_not_decript_without_password()
    {
        var result = await App.RunAsync(["decrypt"]);
        testOutput.WriteLine(App.Console.Output);

        result.ExitCode.Should().NotBe(0);
        App.Console.ShouldHaveOutputThatMatches(@"Error: Password is required to decrypt files\.");
    }

    [Fact]
    public async Task Should_not_decript_with_wrong_password()
    {
        var wrongPassword = password + "x"; // intentionally wrong password
        var result = await App.RunAsync(["decrypt", "-p", wrongPassword]);
        testOutput.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().NotBe(0);
        App.Console.ShouldHaveOutputThatMatches(@"Error: Decryption with password failed\.");
    }


}
