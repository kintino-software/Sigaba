namespace Kintino.CipherConf.Documents.Services.Json;

public class ReplacementTest
{
    [Fact]
    public void Should_get_correct_locations_of_values_with_corresponding_key()
    {
        var originalJson = """
{ "textKey": "text", "number": 49, "array": [1, 2, 3], "objectKey": { "nestedKey": "nestedValue" }, "nullKey": null }
""";

        var service = JsonByteScanner.Create(originalJson);

        service.ValueLocations.Should().BeEquivalentTo(
        [
            new ValueLocation("textKey", 13, 6),
            new ValueLocation("number", 31, 2),
            new ValueLocation("array", 44, 9),
            new ValueLocation("objectKey", 67, 30),
            new ValueLocation("objectKey.nestedKey", 83, 13),
            new ValueLocation("nullKey", 111, 4),
        ]);
    }

    [Fact]
    public void Should_provide_correct_values_to_transform_function()
    {
        var originalJson = """
        {
            "text": "value",
            "number": 49,
            "array": [1, 2, 3],
            "nullKey": null,
            "objectKey": { "nestedKey": "nestedValue" },
        }
        """;
        var expected = new Dictionary<Jsonkey, JsonRawValue>
        {
            [new Jsonkey("text")] = new JsonRawValue("\"value\""),
            [new Jsonkey("number")] = new JsonRawValue("49"),
            [new Jsonkey("array")] = new JsonRawValue("[1, 2, 3]"),
            [new Jsonkey("nullKey")] = new JsonRawValue("null"),
            [new Jsonkey("objectKey")] = new JsonRawValue("{ \"nestedKey\": \"nestedValue\" }"),
            [new Jsonkey("objectKey.nestedKey")] = new JsonRawValue("\"nestedValue\""),
        };
        var service = JsonByteScanner.Create(originalJson);

        var parametersDictionary = new Dictionary<Jsonkey, JsonRawValue>();
        service.Transform((key, rawValue) =>
        {
            parametersDictionary[key] = rawValue;
            return rawValue;
        });

        parametersDictionary.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_transform_values()
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
            "text": "foobar",
            "number": "foobar",
            "array": "foobar",
            "nullKey": "foobar",
            "object": "foobar",
        }
        """;
        var service = JsonByteScanner.Create(original);

        var result = service.Transform((key, rawValue) => new JsonRawValue("\"foobar\""));

        result.Should().Be(expected);
    }


}

