using Json.Path;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Sigaba.Cli.TestHelpers;

public static class Extensions
{
    extension(string str)
    {
        public string AsPath()
        {
            return Path.Combine(str.Split(['/', '\\']));
        }
    }

    extension(IFileSystem fs)
    {
        public JsonTester InspectJson(string filePath)
        {
            var jsonContent = fs.File.ReadAllText(filePath);
            return JsonTester.Parse(jsonContent);
        }

        public void EditJsonFile<T>(string filePath, string jsonPathQuery, Func<T, T> editFunc)
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
}
