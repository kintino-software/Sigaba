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

        service.Results.Should().BeEquivalentTo(
        [
            new ScannerResult("textKey", "\"text\"", 13, 6),
            new ScannerResult("number", "49", 31, 2),
            new ScannerResult("array", "[1, 2, 3]", 44, 9),
            new ScannerResult("objectKey.nestedKey", "\"nestedValue\"", 83, 13),
            new ScannerResult("nullKey", "null", 111, 4),
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
            "objectKey": { "nestedKey": "nestedValue" }
        }
        """;
        ScannerResult[] expected = new ScannerResult[]
        {
            new("text", "\"value\"", 15, 7),
            new("number", "49", 39, 2),
            new("array", "[1, 2, 3]", 57, 9),
            new("nullKey", "null", 84, 4),
            new("objectKey.nestedKey", "\"nestedValue\"", 123, 13),
        };
        var service = JsonByteScanner.Create(originalJson);

        List<ScannerResult> parametersDictionary = new List<ScannerResult>();
        service.Replace(
            (location) =>
            {
                parametersDictionary.Add(location);
                return location.RawValue;
            },
            (_) => true);

        parametersDictionary.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Should_replace_all_values()
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
            "object": { "nestedKey": "foobar" },
        }
        """;
        var service = JsonByteScanner.Create(original);

        var result = service.Replace((location) => "\"foobar\"", (_) => true);

        result.Should().Be(expected);
    }

    [Fact]
    public void Should_replace_filtered_values()
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
            "text": "value",
            "number": 49,
            "array": [1, 2, 3],
            "nullKey": "foobar",
            "object": { "nestedKey": "nestedValue" },
        }
        """;
        var service = JsonByteScanner.Create(original);

        var result = service.Replace((location) => "\"foobar\"", (key) => key == "nullKey");

        result.Should().Be(expected);
    }


}

