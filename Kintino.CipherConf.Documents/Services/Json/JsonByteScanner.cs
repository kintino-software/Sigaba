using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

// Maps a json key to a coordinate (start - end) of it's value in a raw byte array.
internal record JsonValueCoordinate(string Key, int StartIndex, int Length);

// Represents a replacement of a json field value with a new value.
internal record JsonFieldReplacement(string Key, string NewValue);

// Reads a json documet byte by byte and collects the coordinates of each field value in the document.
// It also allows replacing field values with new values without parsing the entire document.
internal class JsonByteScanner
{
    private static JsonReaderOptions readerOptions = new() { CommentHandling = JsonCommentHandling.Allow, AllowTrailingCommas = true };
    private readonly byte[] originalBytes;
    public IReadOnlyDictionary<string, JsonValueCoordinate> KeyToFieldDataMap { get; }

    private JsonByteScanner(byte[] originalBytes, IReadOnlyDictionary<string, JsonValueCoordinate> keyToFieldDataMap)
    {
        this.originalBytes = originalBytes;
        this.KeyToFieldDataMap = keyToFieldDataMap;
    }

    public static JsonByteScanner Create(string jsonDocument)
    {
        var originalBytes = Encoding.UTF8.GetBytes(jsonDocument);
        var replacements = CollectLocations(originalBytes);
        return new JsonByteScanner(originalBytes, replacements);
    }

    public string? GetRawValue(string key)
    {
        if (KeyToFieldDataMap.TryGetValue(key, out var fieldData))
        {
            return Encoding.UTF8.GetString(originalBytes[fieldData.StartIndex..(fieldData.StartIndex + fieldData.Length)]);
        }
        return null;
    }

    public string Replace(params JsonFieldReplacement[] replacements)
    {
        // no changes, so return the original string
        if (KeyToFieldDataMap.Count == 0)
            return Encoding.UTF8.GetString(originalBytes);

        // create a dictionary for quick lookup of replacements by key
        var replacementMap = replacements.ToDictionary(r => r.Key, r => r.NewValue);

        // variable lenght byte collection as changes will likely change the length of the document
        var output = new List<byte>(originalBytes.Length);
        var cursor = 0;

        // Process in order — forward pass, no index shifting issues
        foreach (var coordinate in KeyToFieldDataMap.Values.OrderBy(v => v.StartIndex))
        {
            if (!replacementMap.TryGetValue(coordinate.Key, out var newValue))
            {
                continue;
            }
            output.AddRange(originalBytes[cursor..coordinate.StartIndex]);  // write the original key and other elements to the output
            output.AddRange(Encoding.UTF8.GetBytes(newValue));              // write new value to the output
            cursor = coordinate.StartIndex + coordinate.Length;             // put the cursor after the original value for the next iteration
        }

        output.AddRange(originalBytes[cursor..]); // copy remainder

        return Encoding.UTF8.GetString([.. output]);
    }

    private static Dictionary<string, JsonValueCoordinate> CollectLocations(ReadOnlySpan<byte> utf8Bytes)
    {
        var reader = new Utf8JsonReader(utf8Bytes, readerOptions);
        var result = new Dictionary<string, JsonValueCoordinate>();

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

                    result[currentKey] = new JsonValueCoordinate(
                        Key: currentKey,
                        StartIndex: tokenStart,
                        Length: (tokenEnd - tokenStart)
                    );
                    currentKey = null;
                    break;
            }
        }

        return result;
    }



}
