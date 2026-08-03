namespace Sigaba.Cli.Cases;

public class Init : BaseTest
{
    [Fact]
    public async Task WithoutArgs()
    {
        var cwd = CreateAndSetCwd("a", "b");
        var app = CreateApp();
        await app.RunAsync("init");

        Fs.File.Exists(Fs.Path.Combine(cwd, "private.key")).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(cwd, "public.key")).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(cwd, "sigaba.json")).Should().BeTrue();
    }

}
