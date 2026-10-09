namespace Sigaba.Documents.Services.Json.Parser;

public class JsonParserTest
{
    [Fact]
    public void Should_parse_json()
    {
        // mind line breaks as \r\n, two chars
        var content = """
        {
            "text": "value",
            "number": 49,
            "array": [1, 2, 3],
            "nullKey": null,
            "objectKey": { "nestedKey": "nestedValue" }
        }
        """;

        var tokens = JsonParser.Parse(content).ToArray();

        tokens[0].Should().BeEquivalentTo(new JsonToken("text", "\"value\"", JsonDataType.String, 15, 7));
        tokens[1].Should().BeEquivalentTo(new JsonToken("number", "49", JsonDataType.Number, 39, 2));
        tokens[2].Should().BeEquivalentTo(new JsonToken("array", "[1, 2, 3]", JsonDataType.Array, 57, 9));
        tokens[3].Should().BeEquivalentTo(new JsonToken("nullKey", "null", JsonDataType.Null, 84, 4));
        tokens[4].Should().BeEquivalentTo(new JsonToken("objectKey.nestedKey", "\"nestedValue\"", JsonDataType.String, 123, 13));
    }

    [Fact]
    public void Empty_strings_yield_empty_token_colletion()
    {
        var content = string.Empty;

        var tokens = JsonParser.Parse(content).ToArray();

        tokens.Should().BeEmpty();
    }
}

