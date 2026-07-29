using System.Text;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

public class RawField
{
    public string Key { get; }
    public string RawValue { get; private set; } = string.Empty;

    public RawField(string key, string rawValue)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key), "Invalid json key.");
        }
        Key = key;
        SetRawValueOrThrow(rawValue);
    }

    public void SetRawValue(string rawValue)
    {
        SetRawValueOrThrow(rawValue);
    }

    public bool TryGetValue<T>(out T? value)
    {
        try
        {
            value = JsonSerializer.Deserialize<T>(RawValue);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public void SetValue<T>(T value)
    {
        SetRawValue(JsonSerializer.Serialize(value));
    }

    private void SetRawValueOrThrow(string rawValue)
    {
        if (!IsValidJson(rawValue))
        {
            throw new ArgumentException($"Invalid JSON value for key '{Key}'", nameof(rawValue));
        }
        RawValue = rawValue;
    }

    private static bool IsValidJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var utf8Bytes = Encoding.UTF8.GetBytes(input);
        var reader = new Utf8JsonReader(utf8Bytes);

        try
        {
            reader.Read(); // Move to the first token
            reader.TrySkip(); // Skip the entire structure
            return true;
        }
        catch
        {
            return false;
        }
    }
}
