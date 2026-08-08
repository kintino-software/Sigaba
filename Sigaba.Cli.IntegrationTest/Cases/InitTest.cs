namespace Sigaba.Cli.Cases;

public class InitTest : BaseTest
{
    [Fact]
    public async Task WithoutArgs()
    {
        var cwd = CreateAndSetCwd("a/b".AsPath());
        var app = CreateApp();

        await app.RunAsync("init");

        Fs.File.Exists($"{cwd}/sigaba.json".AsPath()).Should().BeTrue();
        Fs.AllFiles.Should().ContainSingle(f => f.EndsWith("private.key"));
    }

}
