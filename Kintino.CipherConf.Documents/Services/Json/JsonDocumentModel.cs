using Kintino.CipherConf.Documents.Models;
using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    private record Replacement(int Start, int OriginalLength, byte[] NewValueBytes);
    public static DocumentType DocumentType { get; } = DocumentType.Json;
    private JsonReaderOptions readerOptions = new()
    {
        CommentHandling = JsonCommentHandling.Allow,
        AllowTrailingCommas = true,
    };


    public string Transform(string documentContent, Func<IDocumentNode, string> transform)
    {
        var originalBytes = Encoding.UTF8.GetBytes(documentContent);
        var replacements = CollectReplacements(originalBytes, transform);
        var result = ApplyReplacements(originalBytes, replacements);
        return Encoding.UTF8.GetString(result);
    }
    private static List<Replacement> CollectReplacements(
        ReadOnlySpan<byte> utf8Bytes,
        Func<IDocumentNode, string> transform)
    {
        var replacements = new List<Replacement>();
        var reader = new Utf8JsonReader(utf8Bytes, new JsonReaderOptions
        {
            CommentHandling = JsonCommentHandling.Allow,
            AllowTrailingCommas = true,
        });

        string? currentKey = null;
        var keyStack = new Stack<string?>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    var name = reader.GetString()!;
                    currentKey = keyStack.TryPeek(out var parent) && parent is not null
                        ? $"{parent}.{name}"
                        : name;
                    break;

                case JsonTokenType.StartObject:
                    keyStack.Push(currentKey);
                    currentKey = null;
                    break;

                case JsonTokenType.EndObject:
                    keyStack.TryPop(out _);
                    currentKey = null;
                    break;

                case JsonTokenType.StartArray:
                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Null:
                    if (currentKey is null)
                        break;

                    var tokenStart = (int)reader.TokenStartIndex;

                    // For arrays/objects skip the entire block, otherwise we're already past the value
                    if (reader.TokenType == JsonTokenType.StartArray)
                        reader.Skip();

                    var tokenEnd = (int)reader.BytesConsumed;
                    var rawJson = Encoding.UTF8.GetString(utf8Bytes[tokenStart..tokenEnd]);
                    // rawJson examples:
                    //   string  → "\"hello\""
                    //   number  → "42"
                    //   bool    → "true"
                    //   null    → "null"
                    //   array   → "[\"foo\", \"bar\"]"

                    var node = new JsonDocumentNode(currentKey, rawJson);
                    var newRawJson = transform(node);

                    if (newRawJson != rawJson)
                        replacements.Add(new Replacement(
                            tokenStart,
                            tokenEnd - tokenStart,
                            Encoding.UTF8.GetBytes(SanitizeValue(newRawJson))
                        ));

                    currentKey = null;
                    break;
            }
        }

        return replacements;
    }

    private static byte[] ApplyReplacements(byte[] original, List<Replacement> replacements)
    {
        if (replacements.Count == 0)
            return original;

        var output = new List<byte>(original.Length);
        var cursor = 0;

        // Process in order — forward pass, no index shifting issues
        foreach (var r in replacements.OrderBy(r => r.Start))
        {
            output.AddRange(original[cursor..r.Start]); // copy unchanged chunk
            output.AddRange(r.NewValueBytes);           // insert new value
            cursor = r.Start + r.OriginalLength;        // skip old value
        }

        output.AddRange(original[cursor..]);            // copy remainder

        return [.. output];
    }

    private static string SanitizeValue(string value)
    {
        // If the value is a valid JSON, return it as-is; otherwise, treat it as a string and quote it
        try
        {
            using var doc = JsonDocument.Parse(value);
            return value; // It's valid JSON
        }
        catch (JsonException)
        {
            return JsonSerializer.Serialize(value); // Quote it as a string
        }
    }

}
