namespace Sigaba.Cli.IntegrationTest.Cases;

public class EditTest : BaseTest
{
    private readonly string cwd;

    public EditTest()
    {
        InitializeAppAsync().GetAwaiter().GetResult().Deconstruct(out _, out cwd);
    }

    private async Task Encrypt()
    {
        var app = this.CreateCommandApp();
        await app.RunAsync(["encrypt"]);
    }

    [Theory]
    [InlineData("file1.secrets.json")]
    [InlineData(".|file1.secrets.json")]
    [InlineData("subdir1|file1.secrets.json")]
    [InlineData(".|subdir1|file1.secrets.json")]
    public async Task Should_edit_and_encrypt_files(string filePath)
    {
        var file1Path = Fs.Path.Combine(filePath.Split('|'));
        var fileJson = """
            {
                "data_secret": 1234,
                "data_public": "public"
            }
            """;
        Fs.AddFile(file1Path, fileJson);
        await Encrypt();

        var result = await App.RunAsync(["edit", file1Path]);
        TestContext.Current.TestOutputHelper.WriteLine(result.Output);

        result.ExitCode.Should().Be(0);
    }
}
