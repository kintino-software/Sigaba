using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

public record Jsonkey(string Key);
public record JsonRawValue(string Value);
public record ValueLocation(string Key, int StartIndex, int Length);

internal class JsonByteScanner
{
    private static JsonReaderOptions readerOptions = new() { CommentHandling = JsonCommentHandling.Allow, AllowTrailingCommas = true };
    private readonly byte[] originalBytes;
    public IReadOnlyCollection<ValueLocation> ValueLocations { get; }

    private JsonByteScanner(byte[] originalBytes, IReadOnlyCollection<ValueLocation> replacements)
    {
        this.originalBytes = originalBytes;
        this.ValueLocations = replacements;
    }

    public static JsonByteScanner Create(string jsonDocument)
    {
        var originalBytes = Encoding.UTF8.GetBytes(jsonDocument);
        var replacements = CollectLocations(originalBytes);
        return new JsonByteScanner(originalBytes, replacements);
    }

    public string Transform(Func<Jsonkey, JsonRawValue, JsonRawValue> transform)
    {
        if (ValueLocations.Count == 0)
            return Encoding.UTF8.GetString(originalBytes);

        var output = new List<byte>(originalBytes.Length);
        var cursor = 0;

        // Process in order — forward pass, no index shifting issues
        foreach (var r in ValueLocations.OrderBy(r => r.StartIndex))
        {
            var originalValue = Encoding.UTF8.GetString(originalBytes[r.StartIndex..(r.StartIndex + r.Length)]);
            var newValue = transform(new Jsonkey(r.Key), new JsonRawValue(originalValue));
            output.AddRange(originalBytes[cursor..r.StartIndex]); // copy unchanged chunk
            output.AddRange(Encoding.UTF8.GetBytes(newValue.Value));           // insert new value
            cursor = r.StartIndex + r.Length;        // skip old value
        }

        output.AddRange(originalBytes[cursor..]);            // copy remainder

        return Encoding.UTF8.GetString([.. output]);
    }

    private static List<ValueLocation> CollectLocations(ReadOnlySpan<byte> utf8Bytes)
    {
        var replacements = new List<ValueLocation>();
        var reader = new Utf8JsonReader(utf8Bytes, readerOptions);

        string? currentKey = null;
        var keyStack = new Stack<string?>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    var name = reader.GetString()!;
                    if (keyStack.TryPeek(out var parent) && parent is not null)
                    {
                        // as we have a parent key, we set the current key to be the parent key + the current property name
                        currentKey = $"{parent}.{name}";
                    }
                    else
                    {
                        // no parent key, so we just set the current key to be the property name
                        currentKey = name;
                    }
                    break;

                case JsonTokenType.StartObject:
                    // a new object is starting, so we push the current key onto the stack and reset the current key to null
                    keyStack.Push(currentKey);
                    currentKey = null;
                    break;

                case JsonTokenType.EndObject:
                    // an object is ending, so we pop the last key from the stack and set it as the current key 
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

                    replacements.Add(new ValueLocation(
                        currentKey,
                        tokenStart,
                        tokenEnd - tokenStart)
                    );
                    currentKey = null;
                    break;
            }
        }

        return replacements;
    }



}
