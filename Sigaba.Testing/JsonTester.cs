using Json.Path;
using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Sigaba;

/// <summary>
/// A utility class for testing JSON content, allowing for parsing, validation, and editing of JSON data.
/// </summary>
[ExcludeFromCodeCoverage]
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

    /// <summary>
    /// Creates a <see cref="JsonTester"/> from a JSON string.
    /// </summary>
    /// <param name="jsonContent">The JSON content as a string.</param>
    /// <returns>A new instance of <see cref="JsonTester"/>.</returns>
    public static JsonTester FromString(string jsonContent) => new(jsonContent);

    /// <summary>
    /// Creates a <see cref="JsonTester"/> from a <see cref="FilePath"/>.
    /// </summary>
    /// <param name="filePath">The file path to read the JSON content from.</param>
    /// <returns>A new instance of <see cref="JsonTester"/>.</returns>
    public static JsonTester FromFile(FilePath filePath)
    {
        var jsonContent = filePath.Read();
        return new JsonTester(jsonContent);
    }

    /// <summary>
    /// Gets a value from the JSON document using a JSONPath query.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="jsonPath">The JSONPath query string.</param>
    /// <returns>The value retrieved from the JSON document.</returns>
    /// <exception cref="Exception">Thrown if the JSONPath query does not match any value or if the value is not a valid JSON value.</exception>
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

    /// <summary>
    /// Edits a JSON file applying a transformation function to the value at the specified JSONPath query, then saves the file again.
    /// </summary>
    /// <typeparam name="T">The type of the value to edit.</typeparam>
    /// <param name="fs">The file system abstraction.</param>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <param name="jsonPathQuery">The JSONPath query string.</param>
    /// <param name="editFunc">The transformation function to apply to the value.</param>
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

