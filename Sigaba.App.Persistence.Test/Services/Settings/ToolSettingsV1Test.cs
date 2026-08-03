using System.Text.Json.Nodes;

namespace Sigaba.App.Services.Settings;

public class ToolSettingsV1Test : BaseTest
{
    private static ToolSettingsV1 CreateSettings(string fieldRegex = null, string[] includeGlob = null, string[] excludeGlob = null)
    {
        return fieldRegex == null && includeGlob == null && excludeGlob == null
            ? ToolSettingsV1.CreateDefault()
            : new ToolSettingsV1(
                fieldRegex ?? @".*",
                includeGlob ?? [],
                excludeGlob ?? []);
    }

    // Version

    [Fact]
    public void Should_have_version_1()
    {
        var settings = CreateSettings();
        settings.Version.Should().Be(1);
    }

    // Serialization

    [Fact]
    public void Should_serialize_to_json()
    {
        var settings = CreateSettings(
            excludeGlob: ["**/a", "**/b"],
            fieldRegex: @"c$",
            includeGlob: ["**/d", "**/e"]);

        var json = settings.Serialize();

        var root = JsonNode.Parse(json);
        root["version"].GetValue<int>().Should().Be(1);
        root["fieldRegex"].GetValue<string>().Should().Be(@"c$");
        root["include"].AsArray().Select(x => x.GetValue<string>()).Should().BeEquivalentTo(["**/d", "**/e"]);
        root["exclude"].AsArray().Select(x => x.GetValue<string>()).Should().BeEquivalentTo(["**/a", "**/b"]);
    }

    // Deserialize

    [Fact]
    public void Should_create_from_json()
    {
        var json = """
            {
                "version": 1,
                "fieldRegex": "^.*",
                "include": ["**/*"],
                "exclude": ["**/*"]
            }
            """;

        var toolSettings = ToolSettingsV1.Deserialize(json);

        toolSettings.Should().NotBeNull();
    }

    // FieldNamePredicate

    [Fact]
    public void Should_filter_field_names()
    {
        string[] input = ["field1", "field2", "field3_secret", "field4_secret"];
        string[] expected = ["field3_secret", "field4_secret"];
        var settings = CreateSettings(fieldRegex: @"^.*_secret$");

        var result = input.Where(settings.FieldNamePredicate).ToArray();

        result.Should().BeEquivalentTo(expected);
    }

}

