namespace Sigaba.Cli.Cases;

public class InitTest : BaseTest
{
    [Fact]
    public async Task Interactive()
    {
        var cwd = Fs.Directory.GetCurrentDirectory();

        App.Console.Input.PushTextWithEnter("password"); // enter password
        App.Console.Input.PushTextWithEnter("password"); // confirm password
        await App.RunAsync(["init"]);

        Fs.File.Exists(Fs.Path.Combine(cwd, "sigaba.json")).Should().BeTrue();
        Fs.AllFiles.Should().ContainSingle(f => f.EndsWith("private.key"));
    }

    [Theory]
    [InlineData("init", "-n", "-p")]
    [InlineData("init", "--non-interactive", "--password")]
    public async Task NonInteractive(string command, string nonInteractiveOption, string passwordOption)
    {
        var cwd = Fs.Directory.GetCurrentDirectory();

        await App.RunAsync([command, nonInteractiveOption, passwordOption, "password"]);

        Fs.File.Exists(Fs.Path.Combine(cwd, "sigaba.json")).Should().BeTrue();
        Fs.AllFiles.Should().ContainSingle(f => f.EndsWith("private.key"));
    }
}
