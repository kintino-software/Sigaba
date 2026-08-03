using System.Text.Json;

namespace Kintino.CipherConf.App.Services.Settings;

public class SchemaV1Test
{
    [Fact]
    public void Version_must_be_1()
    {
        var obj = new SchemaV1()
        {
            ExcludeFileGlob = [],
            FieldRegex = "",
            IncludeFileGlob = [],
        };

        obj.Version.Should().Be(1);
    }

    [Fact]
    public void Should_serialize_to_json()
    {
        var original = new SchemaV1()
        {
            ExcludeFileGlob = ["**/**/exclude"],
            FieldRegex = @".*__foo__bar$",
            IncludeFileGlob = ["**/**/include"],
        };

        var originalJson = JsonSerializer.Serialize(original);
        var actual = JsonSerializer.Deserialize<SchemaV1>(originalJson);

        originalJson.Should().NotBeNullOrEmpty();
        actual.Should().NotBeNull().And.BeEquivalentTo(original);
    }
}

