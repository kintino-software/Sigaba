namespace Kintino.CipherConf.Cli.Cases;

public class Init : BaseTest
{
    [Fact]
    public async Task WithoutArgs()
    {
        var app = CreateApp();
        await app.RunAsync("init");

        Fs.File.Exists(Fs.Path.Combine(RootPath, PublicKeyFileName)).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, PrivateKeyFileName)).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, ConfigFileName)).Should().BeTrue();
    }

    [Theory]
    [InlineData("-p")]
    [InlineData("--project")]
    public async Task WithProjectArg(string projectArg)
    {
        Fs.Directory.CreateDirectory(Fs.Path.Combine(RootPath, "foobar"));
        var app = CreateApp();
        await app.RunAsync("init", projectArg, "foobar");

        Fs.File.Exists(Fs.Path.Combine(RootPath, "foobar", PublicKeyFileName)).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, "foobar", PrivateKeyFileName)).Should().BeTrue();
        Fs.File.Exists(Fs.Path.Combine(RootPath, "foobar", ConfigFileName)).Should().BeTrue();
    }
}
