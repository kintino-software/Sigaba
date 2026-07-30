using Json.Path;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.TestHelpers;

internal class JsonEvaluator
{
    private readonly JsonNode root;

    private JsonEvaluator(JsonNode root)
    {
        this.root = root;
    }

    public static JsonEvaluator FromFile(MockFileData file)
    {
        return new JsonEvaluator(JsonNode.Parse(file.TextContents));
    }

    public JsonEvaluator AssertValueIs<T>(string jsonPath, T expectedValue)
    {
        var path = JsonPath.Parse(jsonPath);
        var evaluation = path.Evaluate(root);
        if (evaluation.Matches.Count == 0)
        {
            throw new Exception($"Expected at least one match for the JSON path '{jsonPath}', but found none.");
        }
        if (evaluation.Matches[0].Value.GetValue<T>()?.Equals(expectedValue) != true)
        {
            throw new Exception($"Expected the value at JSON path '{jsonPath}' to be {expectedValue} ({expectedValue.GetType()}), but found {evaluation.Matches[0].Value} ({evaluation.Matches[0].Value?.GetType()}).");
        }
        return this;
    }

    public JsonEvaluator AssertValueIsNot<T>(string jsonPath, T expectedValue)
    {
        var path = JsonPath.Parse(jsonPath);
        var evaluation = path.Evaluate(root);
        if (evaluation.Matches.Count == 0)
        {
            throw new Exception($"Expected at least one match for the JSON path '{jsonPath}', but found none.");
        }
        if (evaluation.Matches[0].Value.GetValue<T>()?.Equals(expectedValue) == true)
        {
            throw new Exception($"Expected the value at JSON path '{jsonPath}' to not be '{expectedValue}'.");
        }
        return this;
    }

    public JsonEvaluator AssertHasAnyValue(string jsonPath)
    {
        var path = JsonPath.Parse(jsonPath);
        var evaluation = path.Evaluate(root);
        if (!evaluation.Matches.TryGetSingleValue(out _))
        {
            throw new Exception($"Expected at least one match for the JSON path '{jsonPath}', but found none.");
        }
        return this;
    }
}
