using System.Text;
using System.Text.Json;

namespace Sigaba.Documents.Services.Json.Parser;

internal static class JsonParser
{
    public static IEnumerable<JsonToken> Parse(string jsonContent)
    {
        var bytes = Encoding.UTF8.GetBytes(jsonContent);
        if (bytes.Length == 0)
            return [];

        var reader = new Utf8JsonReader(
            bytes,
            new JsonReaderOptions
            {
                CommentHandling = JsonCommentHandling.Allow,
                AllowTrailingCommas = true
            });

        string? currentKey = null;
        var keyStack = new Stack<string?>();

        var result = new List<JsonToken>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    {
                        var key = reader.GetString()!;
                        if (keyStack.TryPeek(out var lastKey) && lastKey is not null)
                            currentKey = $"{lastKey}.{key}";
                        else
                            currentKey = key;
                        break;
                    }
                case JsonTokenType.StartObject:
                    {
                        keyStack.Push(currentKey);
                        currentKey = null;
                        break;
                    }
                case JsonTokenType.EndObject:
                    {
                        keyStack.TryPop(out _);
                        currentKey = null;
                        break;
                    }

                case JsonTokenType.StartArray:
                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Null:
                    {
                        if (currentKey is null)
                            break;

                        // get type and start index now as it could be an array
                        // and we can skip the entire block below
                        var tokenType = MapJsonTokenType(reader.TokenType);
                        var tokenStart = (int)reader.TokenStartIndex;

                        // For arrays skip the entire block, otherwise we're already past the value
                        // Note: skipping will put the entire array in the same block of data
                        if (reader.TokenType == JsonTokenType.StartArray)
                            reader.Skip();

                        var tokenEnd = (int)reader.BytesConsumed;
                        result.Add(new JsonToken(
                            Path: currentKey,
                            RawValue: jsonContent[tokenStart..tokenEnd],
                            Type: tokenType,
                            StartIndex: tokenStart,
                            Length: (tokenEnd - tokenStart)
                        ));
                        currentKey = null;
                        break;
                    }
            }
        } // end while

        return result.ToArray();
    }

    private static JsonDataType MapJsonTokenType(JsonTokenType tokenType)
    {
        return tokenType switch
        {
            JsonTokenType.String => JsonDataType.String,
            JsonTokenType.Number => JsonDataType.Number,
            JsonTokenType.True => JsonDataType.Boolean,
            JsonTokenType.False => JsonDataType.Boolean,
            JsonTokenType.Null => JsonDataType.Null,
            JsonTokenType.StartArray => JsonDataType.Array,
            _ => throw new NotSupportedException($"Unsupported JSON token type: {tokenType}")
        };
    }
}
