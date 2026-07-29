using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

// Reads a json document byte by byte and collects the coordinates of each field value in the document.
// It also allows replacing field values with new values without reparsing the entire document.
internal class RawJsonSerializer
{
    public record ValuePosition(int StartIndex, int Length);

    private static JsonReaderOptions readerOptions = new() { CommentHandling = JsonCommentHandling.Allow, AllowTrailingCommas = true };
    private readonly JsonSerializerOptions serializerOptions;

    private readonly byte[] originalBytes;
    private readonly Dictionary<string, ValuePosition> keyToPositionMap;
    private readonly Dictionary<string, string> replacements;
    private readonly Dictionary<string, string> additions;

    public IReadOnlyDictionary<string, ValuePosition> KeyToPositionMap => keyToPositionMap;
    public IReadOnlyDictionary<string, string> Replacements => replacements;
    public IReadOnlyDictionary<string, string> Additions => additions;

    // instantiation

    private RawJsonSerializer(byte[] originalBytes, Dictionary<string, ValuePosition> keyToPositionMap)
    {
        serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            IndentSize = 4,
        };
        this.originalBytes = originalBytes;
        this.keyToPositionMap = keyToPositionMap;
        this.replacements = [];
        this.additions = [];
    }

    public static RawJsonSerializer Create(string jsonDocument)
    {
        var originalBytes = Encoding.UTF8.GetBytes(jsonDocument);
        var replacements = CollectPositions(originalBytes);
        return new RawJsonSerializer(originalBytes, replacements);
    }

    // query

    public bool TryGetRawValue(string key, [NotNullWhen(true)] out string? value)
    {
        if (keyToPositionMap.TryGetValue(key, out var fieldData))
        {
            value = Encoding.UTF8.GetString(originalBytes[fieldData.StartIndex..(fieldData.StartIndex + fieldData.Length)]);
            return true;
        }
        value = null;
        return false;
    }

    public bool TryGetValue<T>(string key, [NotNullWhen(true)] out T? value)
    {
        var result = false;
        value = default;
        if (TryGetRawValue(key, out var rawValue))
        {
            result = true;
            value = rawValue == "null" ? default : JsonSerializer.Deserialize<T>(rawValue, serializerOptions);
        }
        return result;
    }

    // modification

    public RawJsonSerializer Replace(string key, string newRawValue)
    {
        if (!keyToPositionMap.ContainsKey(key))
            throw new ArgumentException($"Key '{key}' does not exist in the original document.", nameof(key));
        replacements.Add(key, newRawValue);
        return this;
    }

    public RawJsonSerializer Replace<T>(string key, T newValue)
    {
        var rawValue = newValue is null ? "null" : JsonSerializer.Serialize(newValue, serializerOptions);
        return Replace(key, rawValue);
    }

    public RawJsonSerializer AppendToRoot(string key, string newRawValue)
    {
        if (keyToPositionMap.ContainsKey(key))
            throw new ArgumentException($"Key '{key}' already exists in the original document.", nameof(key));
        additions.Add(key, newRawValue);
        return this;
    }

    public RawJsonSerializer AppendToRoot<T>(string key, T newValue)
    {
        var rawValue = newValue is null ? "null" : JsonSerializer.Serialize(newValue, serializerOptions);
        return AppendToRoot(key, rawValue);
    }

    public RawJsonSerializer AppendToRootOrReplace(string key, string newRawValue)
    {
        if (keyToPositionMap.ContainsKey(key))
            return Replace(key, newRawValue);
        else
            return AppendToRoot(key, newRawValue);
    }

    public RawJsonSerializer AppendToRootOrReplace<T>(string key, T newValue)
    {
        var rawValue = newValue is null ? "null" : JsonSerializer.Serialize(newValue, serializerOptions);
        return AppendToRootOrReplace(key, rawValue);
    }

    // serialization

    public string Serialize()
    {
        var modified = ReplaceAll(originalBytes, keyToPositionMap, replacements);
        var appended = Append(modified, additions);
        return appended;
    }

    // helpers

    private static string Append(string json, Dictionary<string, string> additions)
    {
        if (additions.Count == 0)
            return json;

        var lastClosingBraceIndex = json.LastIndexOf('}');
        if (lastClosingBraceIndex == -1)
            throw new InvalidOperationException("Invalid JSON document: no closing brace found.");

        var sb = new StringBuilder(json.Length + additions.Sum(kv => kv.Key.Length + kv.Value.Length + 6)); // rough estimate
        sb.Append(json.AsSpan(0, lastClosingBraceIndex).TrimEnd());

        foreach (var (key, value) in additions)
        {
            sb.Append(',');
            sb.Append(Environment.NewLine);
            sb.Append("    ");
            sb.Append('"');
            sb.Append(key);
            sb.Append("\": ");
            sb.Append(value);
        }

        sb.Append(Environment.NewLine);
        sb.Append(json.AsSpan(lastClosingBraceIndex));
        return sb.ToString();
    }

    private static string ReplaceAll(byte[] originalBytes, Dictionary<string, ValuePosition> positions, Dictionary<string, string> replacements)
    {
        if (replacements.Count == 0)
            return Encoding.UTF8.GetString(originalBytes);

        var output = new List<byte>(originalBytes.Length);
        var cursor = 0;
        foreach (var (key, coordinate) in positions.OrderBy(kv => kv.Value.StartIndex))
        {
            if (!replacements.TryGetValue(key, out var newValue))
                continue;
            output.AddRange(originalBytes[cursor..coordinate.StartIndex]);  // write the original key and other elements to the output
            output.AddRange(Encoding.UTF8.GetBytes(newValue));              // write new value to the output
            cursor = coordinate.StartIndex + coordinate.Length;             // put the cursor after the original value for the next iteration
        }
        output.AddRange(originalBytes[cursor..]); // copy remainder
        return Encoding.UTF8.GetString([.. output]);
    }

    private static Dictionary<string, ValuePosition> CollectPositions(ReadOnlySpan<byte> utf8Bytes)
    {
        var reader = new Utf8JsonReader(utf8Bytes, readerOptions);
        var result = new Dictionary<string, ValuePosition>();

        string? currentKey = null;
        var keyStack = new Stack<string?>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.PropertyName:
                    {
                        var name = reader.GetString()!;
                        if (keyStack.TryPeek(out var parent) && parent is not null)
                            currentKey = $"{parent}.{name}";
                        else
                            currentKey = name;
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

                        var tokenStart = (int)reader.TokenStartIndex;

                        // For arrays/objects skip the entire block, otherwise we're already past the value
                        if (reader.TokenType == JsonTokenType.StartArray)
                            reader.Skip();

                        var tokenEnd = (int)reader.BytesConsumed;

                        result[currentKey] = new ValuePosition(
                            StartIndex: tokenStart,
                            Length: (tokenEnd - tokenStart)
                        );
                        currentKey = null;
                        break;
                    }
            }
        }
        return result;
    }
}
