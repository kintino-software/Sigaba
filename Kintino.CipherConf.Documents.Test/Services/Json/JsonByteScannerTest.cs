namespace Kintino.CipherConf.Documents.Services.Json;

public class JsonByteScannerTest
{
    [Fact]
    public void Should_get_correct_locations_of_values_with_corresponding_key()
    {
        var originalJson = """
{ "textKey": "text", "number": 49, "array": [1, 2, 3], "objectKey": { "nestedKey": "nestedValue" }, "nullKey": null }
""";

        var service = JsonByteScanner.Create(originalJson);

        service.KeyToFieldDataMap.Should().BeEquivalentTo(new Dictionary<string, JsonFieldData>()
        {
            ["textKey"] = new JsonFieldData("textKey", 13, 6),
            ["number"] = new JsonFieldData("number", 31, 2),
            ["array"] = new JsonFieldData("array", 44, 9),
            ["objectKey.nestedKey"] = new JsonFieldData("objectKey.nestedKey", 83, 13),
            ["nullKey"] = new JsonFieldData("nullKey", 111, 4),
        });
    }

    [Fact]
    public void Should_get_raw_values()
    {
        var originalJson = """
        {
            "text": "value",
            "number": 49,
            "array": [1, 2, 3],
            "nullKey": null,
            "objectKey": { "nestedKey": "nestedValue" }
        }
        """;
        var service = JsonByteScanner.Create(originalJson);

        service.GetRawValue("text").Should().Be(@"""value""");
        service.GetRawValue("number").Should().Be("49");
        service.GetRawValue("array").Should().Be("[1, 2, 3]");
        service.GetRawValue("nullKey").Should().Be("null");
        service.GetRawValue("objectKey.nestedKey").Should().Be(@"""nestedValue""");
    }

    [Fact]
    public void Should_return_null_when_getting_inexistent_key_value()
    {
        var originalJson = """
        {
            "text": "value",
            "number": 49,
            "array": [1, 2, 3],
            "nullKey": null,
            "objectKey": { "nestedKey": "nestedValue" }
        }
        """;
        var service = JsonByteScanner.Create(originalJson);

        service.GetRawValue("inexistentKey").Should().BeNull();
    }

    [Fact]
    public void Should_replace_raw_values()
    {
        var original = """
        {
            "text": "value",
            "number": 49,
            "array": [1, 2, 3],
            "nullKey": null,
            "object": { "nestedKey": "nestedValue" },
        }
        """;
        var expected = """
        {
            "text": null,
            "number": "foobar",
            "array": "foobar",
            "nullKey": "foobar",
            "object": { "nestedKey": ["foo", "bar"] },
        }
        """;
        var service = JsonByteScanner.Create(original);

        var result = service.Replace(
            new JsonFieldReplacement("text", "null"),
            new JsonFieldReplacement("array", @"""foobar"""),
            new JsonFieldReplacement("number", @"""foobar"""),
            new JsonFieldReplacement("nullKey", @"""foobar"""),
            new JsonFieldReplacement("object.nestedKey", @"[""foo"", ""bar""]"));

        result.Should().Be(expected);
    }

}

