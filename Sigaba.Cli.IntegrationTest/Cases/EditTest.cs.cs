using System.IO.Abstractions.TestingHelpers;
using Xunit.Abstractions;

namespace Sigaba.Cli.IntegrationTest.Cases;

public class EditTest : BaseTest
{
    private readonly string cwd;
    private readonly ITestOutputHelper testOutput;

    public EditTest(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
        InitializeAppAsync().GetAwaiter().GetResult().Deconstruct(out _, out cwd);
    }

    private async Task Encrypt()
    {
        var app = this.CreateCommandApp();
        await app.RunAsync(["encrypt"]);
    }

    [Theory]
    [InlineData("file1.secrets.json", @"{""foo"": ""bar""}")]
    [InlineData("dir/file1.secrets.json", @"{""foo"": ""bar""}")]
    [InlineData("dir/subdir/file1.secrets.json", @"{""foo"": ""bar""}")]
    [InlineData(".env", "KEY=value")]
    [InlineData("dir/.env", "KEY=value")]
    [InlineData("dir/subdir/.env", "KEY=value")]
    public async Task Should_edit_and_encrypt_files(string filePath, string content)
    {
        var file1Path = Fs.Path.Combine(filePath);
        var fileJson = content;
        Fs.AddFile(file1Path, new MockFileData(fileJson));
        await Encrypt();

        var result = await App.RunAsync(["edit", file1Path]);
        testOutput.WriteLine(result.Output);

        result.ExitCode.Should().Be(0);
    }
}
