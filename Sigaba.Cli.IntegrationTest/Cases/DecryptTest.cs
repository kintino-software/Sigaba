using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.Cli.IntegrationTest.Cases;

public class DecryptTest : BaseTest
{
    private readonly string cwd;
    private readonly string password;

    public DecryptTest()
    {
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
        var path1 = Fs.Path.Combine(cwd, "file1.secrets.json");
        var originalContent1 = """
            {
                "field1": "value 1",
                "field2_secret": "secret value 2",
            }
            """;
        Fs.AddFile(path1, new MockFileData(originalContent1));

        var path2 = Fs.Path.Combine(cwd, "file2.secrets.json");
        var originalContent2 = """
            {
                "field3": "value 3",
                "field4_secret": "secret value 4",
            }
            """;
        Fs.AddFile(path2, new MockFileData(originalContent2));

        await Encrypt();

        //

        var result = await App.RunAsync([command, passwordArg, password]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().Be(0);
        Fs.GetFile(path1).TextContents.Should().Be(originalContent1);
        Fs.GetFile(path2).TextContents.Should().Be(originalContent2);
        App.Console.ShouldHaveOutputThatMatches("""
            ^2 file\(s\) affected:$
            ^\s\s.*file1\.secrets\.json$
            ^\s\s.*file2\.secrets\.json$
            """);
    }

    [Fact]
    public async Task Should_not_decript_without_private_key()
    {
        Fs.RemoveFile(Fs.AllFiles.First(f => f.EndsWith("private.key"))); // remove private key to simulate missing key

        //

        var result = await App.RunAsync(["decrypt", "-p", password]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().NotBe(0);
        App.Console.ShouldHaveOutputThatMatches(@"Error: Private key not found on any of expected locations\.");
    }

    [Fact]
    public async Task Should_not_decript_without_password()
    {
        var result = await App.RunAsync(["decrypt"]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        result.ExitCode.Should().NotBe(0);
        App.Console.ShouldHaveOutputThatMatches(@"Error: Password is required to decrypt files\.");
    }

    [Fact]
    public async Task Should_not_decript_with_wrong_password()
    {
        var wrongPassword = password + "x"; // intentionally wrong password
        var result = await App.RunAsync(["decrypt", "-p", wrongPassword]);
        TestContext.Current.TestOutputHelper.WriteLine(App.Console.Output);

        //

        result.ExitCode.Should().NotBe(0);
        App.Console.ShouldHaveOutputThatMatches(@"Error: Decryption with password failed\.");
    }
}
