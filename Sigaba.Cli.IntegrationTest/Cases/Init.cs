namespace Sigaba.Cli.Cases;

public class Init : BaseTest
{
    [Fact]
    public async Task WithoutArgs()
    {
        var app = CreateApp();
        await app.RunAsync("init");

        Fs.File.Exists(Fs.Path.Combine(RootPath, "private.key")).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, "public.key")).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, "cipherconf.settings.json")).Should().BeTrue();
    }

    [Theory]
    [InlineData("-p")]
    [InlineData("--project")]
    public async Task WithProjectArg(string projectArg)
    {
        Fs.Directory.CreateDirectory(Fs.Path.Combine(RootPath, "foobar"));
        var app = CreateApp();
        await app.RunAsync("init", projectArg, "foobar");

        Fs.File.Exists(Fs.Path.Combine(RootPath, "foobar", "public.key")).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, "foobar", "private.key")).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, "foobar", "cipherconf.settings.json")).Should().BeTrue();
    }
}
