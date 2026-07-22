using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

internal record ScannerResult(FieldKey Key, FieldRawValue RawValue, int StartIndex, int Length);

internal class JsonByteScanner
{
    private static JsonReaderOptions readerOptions = new() { CommentHandling = JsonCommentHandling.Allow, AllowTrailingCommas = true };
    private readonly byte[] originalBytes;
    public IReadOnlyCollection<ScannerResult> Results { get; }

    private JsonByteScanner(byte[] originalBytes, IReadOnlyCollection<ScannerResult> replacements)
    {
        this.originalBytes = originalBytes;
        this.Results = replacements;
    }

    public static JsonByteScanner Create(string jsonDocument)
    {
        var originalBytes = Encoding.UTF8.GetBytes(jsonDocument);
        var replacements = CollectLocations(originalBytes);
        return new JsonByteScanner(originalBytes, replacements);
    }

    public string Replace(Func<ScannerResult, FieldRawValue> transform, Func<FieldKey, bool> predicate)
    {
        if (Results.Count == 0)
            return Encoding.UTF8.GetString(originalBytes);

        var output = new List<byte>(originalBytes.Length);
        var cursor = 0;

        // Process in order — forward pass, no index shifting issues
        foreach (var r in Results.OrderBy(r => r.StartIndex))
        {
            if (predicate(new FieldKey(r.Key)))
            {
                var newValue = transform(r);
                output.AddRange(originalBytes[cursor..r.StartIndex]);       // copy unchanged chunk
                output.AddRange(Encoding.UTF8.GetBytes(newValue.Value));    // insert new value
                cursor = r.StartIndex + r.Length;                           // skip old value
            }
        }

        output.AddRange(originalBytes[cursor..]); // copy remainder

        return Encoding.UTF8.GetString([.. output]);
    }

    private static List<ScannerResult> CollectLocations(ReadOnlySpan<byte> utf8Bytes)
    {
        var locations = new List<ScannerResult>();
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

                    locations.Add(new ScannerResult(
                        Key: currentKey,
                        RawValue: Encoding.UTF8.GetString(utf8Bytes[tokenStart..tokenEnd]),
                        StartIndex: tokenStart,
                        Length: tokenEnd - tokenStart)
                    );
                    currentKey = null;
                    break;
            }
        }

        return locations;
    }



}
