namespace Sigaba.App.Services.SigabaFiles;

public class JsonHelperTest
{
    // ReadVersionFromJson

    [Fact]
    public void ReadVersionFromJson_should_return_version_field_value()
    {
        var json = """
            {
                "version": 99
            }
            """;

        JsonHelper.ReadVersionFromJson(json).Should().Be(99);
    }

    [Fact]
    public void ReadVersionFromJson_should_return_minus_1_when_version_field_does_not_exist()
    {
        var json = """
            {
                "not_version": 99
            }
            """;

        JsonHelper.ReadVersionFromJson(json).Should().Be(-1);
    }

    [Fact]
    public void ReadVersionFromJson_should_return_minus_1_when_json_is_malformed()
    {
        var json = """
            
                malformed json
            }
            """;

        JsonHelper.ReadVersionFromJson(json).Should().Be(-1);
    }
}

