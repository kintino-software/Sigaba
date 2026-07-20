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
                    "field1": "public-value",
                    "field2_secret": "private-value",
                    "field3_secret": 1,
                    "field4_secret": true,
                    "field5_secret": null,
                    "field6_secret": ["a", "b", "c"]
                }
            """));
        await InitProject();
        var app = CreateApp();

        //

        await app.RunAsync("encrypt");

        //

        (await GetPropertyFromJsonDocument(settingsFilePath, "field1")).Should().Be("public-value");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field2_secret")).Should().NotBe("private-value");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field3_secret")).Should().NotBe("1");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field4_secret")).Should().NotBe("true");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field5_secret")).Should().NotBe("null");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field6_secret")).Should().NotBe("""["a", "b", "c"]""");
    }

    [Fact]
    public async Task DecryptFlow()
    {
        var settingsFilePath = Fs.Path.Combine(RootPath, "appsettings.json");

        Fs.AddFile(
            settingsFilePath,
            new MockFileData("""
                {
                    "field1": "public-value",
                    "field2_secret": "private-value",
                    "field3_secret": 1,
                    "field4_secret": true
                }
            """));
        await InitProject();
        var app = CreateApp();
        await app.RunAsync("encrypt");

        //

        await app.RunAsync("decrypt");

        //

        (await GetPropertyFromJsonDocument(settingsFilePath, "field1")).Should().Be("public-value");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field2_secret")).Should().Be("private-value");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field3_secret")).Should().Be("1");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field4_secret")).Should().Be("true");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field5_secret")).Should().Be("null");
        (await GetPropertyFromJsonDocument(settingsFilePath, "field6_secret")).Should().Be("""["a", "b", "c"]""");
    }
}
