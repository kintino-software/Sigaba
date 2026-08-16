using Json.Path;
using Sigaba.Primitives;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Sigaba;

public class JsonTester
{
  private readonly string jsonContent;
  private readonly Lazy<JsonNode> lazyJsonNode;

  private JsonTester(string jsonContent)
  {
    this.jsonContent = jsonContent;
    lazyJsonNode = new Lazy<JsonNode>(() => JsonNode.Parse(
        jsonContent,
        documentOptions: new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip }));
  }

  public static JsonTester FromString(string jsonContent) => new(jsonContent);

  public static JsonTester FromFile(FilePath filePath)
  {
    var jsonContent = filePath.Read();
    return new JsonTester(jsonContent);
  }

  public T GetJsonValue<T>(string jsonPath)
  {
    var jsonPathResult = JsonPath.Parse(jsonPath).Evaluate(lazyJsonNode.Value);
    if (jsonPathResult.Matches.Count == 0)
    {
      throw new Exception($"Could not parse '{jsonPath}' from json content:\n{jsonContent}.");
    }
    var match = jsonPathResult.Matches[0];
    var jsonValue = match.Value.AsValue()
        ?? throw new Exception($"The value at '{jsonPath}' is not a valid JSON value.");
    return jsonValue.GetValue<T>();
  }

  public void AssertIsValidJson(bool allowTrailingCommas = true, JsonCommentHandling commentHandling = JsonCommentHandling.Allow)
  {
    var reader = new Utf8JsonReader(
        Encoding.UTF8.GetBytes(jsonContent),
        new JsonReaderOptions
        {
          CommentHandling = commentHandling,
          AllowTrailingCommas = allowTrailingCommas,
        });
    while (reader.Read()) { /* just reading to validate */ }
  }

  public static void EditJsonFileInPlace<T>(IFileSystem fs, string filePath, string jsonPathQuery, Func<T, T> editFunc)
  {
    if (!fs.File.Exists(filePath))
    {
      throw new FileNotFoundException($"The file '{filePath}' was not found.");
    }

    var content = fs.File.ReadAllText(filePath);
    var rootNode = JsonNode.Parse(content);
    var jsonPathResult = JsonPath.Parse(jsonPathQuery).Evaluate(rootNode);

    if (jsonPathResult.Matches.Count == 0)
    {
      throw new Exception($"Could not parse '{jsonPathQuery}' from json content:\n{content}.");
    }

    foreach (var match in jsonPathResult.Matches)
    {
      var jsonValue = match.Value.AsValue()
          ?? throw new Exception($"The value at '{jsonPathQuery}' is not a valid JSON value.");
      var currentValue = jsonValue.GetValue<T>();
      var newValue = editFunc(currentValue);

      jsonValue.ReplaceWith(JsonValue.Create(newValue));
    }

    fs.File.WriteAllText(filePath, rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
  }
}

