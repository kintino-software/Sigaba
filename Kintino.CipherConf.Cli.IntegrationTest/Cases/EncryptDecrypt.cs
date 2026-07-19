using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Cli.Cases;

public class EncryptDecrypt : BaseTest
{
    private async Task InitProject()
    {
        await CreateApp().RunAsync("init");
    }

    [Fact]
    public async Task EncryptFlow()
    {
        var settingsFilePath = Fs.Path.Combine(RootPath, "appsettings.json");

        Fs.AddFile(
            settingsFilePath,
            new MockFileData("""
                {
                    "public-field": "public-value",
                    "private-field_secret": "private-value"
                }
            """));
        await InitProject();
        var app = CreateApp();

        //

        await app.RunAsync("encrypt");

        //

        (await GetPropertyFromJsonDocument(settingsFilePath, "public-field")).Should().Be("public-value");
        (await GetPropertyFromJsonDocument(settingsFilePath, "private-field_secret")).Should().NotBe("private-value");
    }

    [Fact]
    public async Task DecryptFlow()
    {
        var settingsFilePath = Fs.Path.Combine(RootPath, "appsettings.json");

        Fs.AddFile(
            settingsFilePath,
            new MockFileData("""
                {
                    "public-field": "public-value",
                    "private-field_secret": "private-value"
                }
            """));
        await InitProject();
        var app = CreateApp();
        await app.RunAsync("encrypt");

        //

        await app.RunAsync("decrypt");

        //

        (await GetPropertyFromJsonDocument(settingsFilePath, "public-field")).Should().Be("public-value");
        (await GetPropertyFromJsonDocument(settingsFilePath, "private-field_secret")).Should().Be("private-value");
    }
}
