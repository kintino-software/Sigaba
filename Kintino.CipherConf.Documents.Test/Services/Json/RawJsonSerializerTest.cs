using Xunit.Abstractions;

namespace Kintino.CipherConf.Documents.Services.Json;

public class RawJsonSerializerTest(ITestOutputHelper output)
{
    // Create

    [Fact]
    public void Should_get_correct_locations_of_values_with_corresponding_key()
    {
        var originalJson = """
{ "textKey": "text", "number": 49, "array": [1, 2, 3], "objectKey": { "nestedKey": "nestedValue" }, "nullKey": null }
""";

        var service = RawJsonSerializer.Create(originalJson);

        service.KeyToPositionMap.Should().BeEquivalentTo(new Dictionary<string, RawJsonSerializer.ValuePosition>()
        {
            ["textKey"] = new(13, 6),
            ["number"] = new(31, 2),
            ["array"] = new(44, 9),
            ["objectKey.nestedKey"] = new(83, 13),
            ["nullKey"] = new(111, 4),
        });
    }

    // GetRawValue

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
        var service = RawJsonSerializer.Create(originalJson);

        service.TryGetRawValue("text", out var textValue).Should().BeTrue();
        textValue.Should().Be(@"""value""");
        service.TryGetRawValue("number", out var numberValue).Should().BeTrue();
        numberValue.Should().Be("49");
        service.TryGetRawValue("array", out var arrayValue).Should().BeTrue();
        arrayValue.Should().Be("[1, 2, 3]");
        service.TryGetRawValue("nullKey", out var nullValue).Should().BeTrue();
        nullValue.Should().Be("null");
        service.TryGetRawValue("objectKey.nestedKey", out var nestedValue).Should().BeTrue();
        nestedValue.Should().Be(@"""nestedValue""");
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
        var service = RawJsonSerializer.Create(originalJson);

        service.TryGetRawValue("inexistentKey", out var inexistentValue).Should().BeFalse();
        inexistentValue.Should().BeNull();
    }

    // Replace

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
        var service = RawJsonSerializer.Create(original);

        service
            .Replace("text", "null")
            .Replace("array", @"""foobar""")
            .Replace("number", @"""foobar""")
            .Replace("nullKey", @"""foobar""")
            .Replace("object.nestedKey", @"[""foo"", ""bar""]");


        service.Serialize().Should().Be(expected);
    }

    [Fact]
    public void Should_keep_original_content_format_when_changing_values()
    {
        // in this test, mind the invisible spaces and newlines in the original and expected jsons,
        var original = """
        {
        "text": "value",
                    "number": 49,
            "array": [1, 2, 3],
                "nullKey": null,
                // comment abc
            "object": { 
                "nestedKey": "nestedValue" 
            },
        }
        """;
        var expected = """
        {
        "text": null,
                    "number": "foobar",
            "array": "foobar",
                "nullKey": "foobar",
                // comment abc
            "object": { 
                "nestedKey": [
                    "foo",
                    "bar"
                ] 
            },
        }
        """;
        var service = RawJsonSerializer.Create(original);

        service
            .Replace("text", "null")
            .Replace("array", @"""foobar""")
            .Replace("number", @"""foobar""")
            .Replace("nullKey", @"""foobar""")
            .Replace("object.nestedKey", """
            [
                        "foo",
                        "bar"
                    ]
            """);

        var result = service.Serialize();
        result.Should().Be(expected);
    }

    // AppendToRoot

    [Fact]
    public void Should_append_new_field_to_root()
    {
        var original = """
        {
            "text": "value",
            "number": 49
        }
        """;
        var expected = """
        {
            "text": "value",
            "number": 49,
            "newField": "newValue"
        }
        """;
        var service = RawJsonSerializer.Create(original);

        service.AppendToRoot("newField", @"""newValue""");

        var result = service.Serialize();
        output.WriteLine(result);
        result.Should().Be(expected);
    }

}

