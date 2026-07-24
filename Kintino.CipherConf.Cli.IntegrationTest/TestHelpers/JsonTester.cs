using System.Text.Json;

namespace Kintino.CipherConf.Cli.TestHelpers;

public class JsonTester
{
    private readonly JsonDocument document;

    private JsonTester(JsonDocument document) => this.document = document;

    public static JsonTester Parse(string jsonString)
    {
        var document = JsonDocument.Parse(
            jsonString,
            new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
        return new JsonTester(document);
    }

    public JsonTester ShouldHavePropertyWithValue(string propertyName, string expectedValueAsString)
    {
        if (!TryGetValue(propertyName, out var value))
        {
            throw new Exception($"Property '{propertyName}' is undefined in JSON document.");
        }
        if (expectedValueAsString != null && value != expectedValueAsString)
        {
            throw new Exception($"Property '{propertyName}' has value '{value}', expected '{expectedValueAsString}'.");
        }
        return this;
    }

    public JsonTester ShouldHavePropertyWithValueThatIsNot(string propertyName, string expectedValueAsString)
    {
        if (!TryGetValue(propertyName, out var value))
        {
            throw new Exception($"Property '{propertyName}' is undefined in JSON document.");
        }
        if (expectedValueAsString != null && value == expectedValueAsString)
        {
            throw new Exception($"Property '{propertyName}' has value '{value}', which is not expected.");
        }
        return this;
    }

    public bool TryGetValue(string propertyName, out string valueAsString)
    {
        valueAsString = null;
        if (!document.RootElement.TryGetProperty(propertyName, out var propertyValue))
        {
            return false;
        }

        if (propertyValue.ValueKind == JsonValueKind.Undefined)
        {
            return false;
        }

        valueAsString = propertyValue.ValueKind == JsonValueKind.Null ? "null" : JsonSerializer.Serialize(propertyValue);
        return true;
    }
}