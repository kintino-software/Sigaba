namespace Sigaba.Cli.Cases;

public class InitTest : BaseTest
{
    [Fact]
    public async Task WithoutArgs()
    {
        var privateKeyDir = Fs.SafePath("a/b");
        Fs.AddDirectory(privateKeyDir);
        var cwd = Fs.SafeSetCwd(Fs.SafePath("cwd"));

        App.Console.Input.PushTextWithEnter("password"); // enter password
        App.Console.Input.PushTextWithEnter("password"); // confirm password
        App.Console.Input.PushTextWithEnter(privateKeyDir); // fileLocation
        await App.RunAsync(["init"]);

        Fs.File.Exists(Fs.SafePath(cwd, "sigaba.json")).Should().BeTrue();
        Fs.AllFiles.Should().ContainSingle(f => f.EndsWith("private.key"));
    }

}
