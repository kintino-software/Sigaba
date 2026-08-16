namespace Sigaba.Cli.Cases;

public class InitTest : BaseTest
{
    [Fact]
    public async Task WithoutArgs()
    {
        var cwd = Fs.Directory.GetCurrentDirectory();

        App.Console.Input.PushTextWithEnter("password"); // enter password
        App.Console.Input.PushTextWithEnter("password"); // confirm password
        await App.RunAsync(["init"]);

        Fs.File.Exists(Fs.Path.Combine(cwd, "sigaba.json")).Should().BeTrue();
        Fs.AllFiles.Should().ContainSingle(f => f.EndsWith("private.key"));
    }

}
