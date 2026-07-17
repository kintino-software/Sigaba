using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

public class JsonTraverserTest
{
    [Fact]
    public void Should_return_json_entries()
    {
        var input = """
            {
              "prop0": "top0 value",
              "prop1": 1,
              "prop2": {
                "prop3": "prop3 value"
              },
              "prop4": [
                "prop4 value 0",
                "prop4 value 1"
              ],
              "prop5": true
            }
            """;
        var root = JsonNode.Parse(input);

        //

        var result = JsonTraverser.Traverse(root);

        //

        result.Should().NotBeNull();

    }


}

