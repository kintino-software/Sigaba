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

        Fs.InspectJson(settingsFilePath)
            .ShouldHavePropertyWithValue("field1", "\"public-value\"")
            .ShouldHavePropertyWithValueThatIsNot("field2_secret", "\"private-value\"")
            .ShouldHavePropertyWithValueThatIsNot("field3_secret", "1")
            .ShouldHavePropertyWithValueThatIsNot("field4_secret", "true")
            .ShouldHavePropertyWithValueThatIsNot("field5_secret", "null")
            .ShouldHavePropertyWithValueThatIsNot("field6_secret", """["a", "b", "c"]""");
    }

    [Fact]
    public async Task DecryptFlow()
    {
        var settingsFilePath = Fs.Path.Combine(RootPath, "appsettings.json");
        var originalContent = """
        {
            "field1": "public-value",
            "field2_secret": "private-value",
            "field3_secret": 1,
            "field4_secret": true,
            "field5_secret": null,
            "field6_secret": [
                "a", 
                "b", 
                "c"
            ]
        }
        """;

        Fs.AddFile(settingsFilePath, new MockFileData(originalContent));
        await InitProject();
        var app = CreateApp();
        await app.RunAsync("encrypt");

        //

        await app.RunAsync("decrypt");

        //

        Fs.GetFile(settingsFilePath).TextContents.Should().Be(originalContent);
    }
}
