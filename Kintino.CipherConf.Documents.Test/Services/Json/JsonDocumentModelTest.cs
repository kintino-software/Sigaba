namespace Kintino.CipherConf.Documents.Services.Json;

public class JsonDocumentModelTest
{
    // Deserialize

    [Fact]
    public void Should_deserialize_any_value_kind()
    {
        var json = """
        {
            "textKey": "text",
            "numberKey": 123,
            "booleanKey": true,
            "arrayKey": [1, 2, 3],
            "objectKey": {
                "nestedKey": "nestedValue"
            },
            "nullKey": null
        }
        """;
        var model = new JsonDocumentModel();

        model.Deserialize(json);

        model.RootNode.Should().NotBeNull();
        model.RootNode["textKey"].GetValue<string>().Should().Be("text");
        model.RootNode["numberKey"].GetValue<int>().Should().Be(123);
        model.RootNode["booleanKey"].GetValue<bool>().Should().BeTrue();
        model.RootNode["arrayKey"].AsArray().Should().HaveCount(3);
        model.RootNode["objectKey"]["nestedKey"].GetValue<string>().Should().Be("nestedValue");
        model.RootNode["nullKey"].Should().BeNull(); // nodes of null data kind is represented as null
    }

    [Theory]
    [InlineData("{key:\"value\"}")]
    [InlineData("")]
    [InlineData(null)]
    public void Should_throw_exception_when_deserializing_invalid_json(string invalidJson)
    {
        var model = new JsonDocumentModel();

        Action act = () => model.Deserialize(invalidJson);

        act.Should().Throw<InvalidOperationException>();
    }

    // Serialize

    [Fact]
    public void Should_serialize_json_document()
    {
        var json = """
        {
            "textKey": "text",
            "numberKey": 123,
            "booleanKey": true,
            "arrayKey": [1, 2, 3],
            "objectKey": {
                "nestedKey": "nestedValue"
            },
            "nullKey": null
        }
        """;
        var model = new JsonDocumentModel();
        model.Deserialize(json);

        var result = model.Serialize();

        result.Should().Be(json);
    }

    // GetNodes

    [Fact]
    public void Should_get_nodes()
    {
        var json = """
        {
            "textKey": "text",
            "numberKey": 123,
            "booleanKey": true,
            "arrayKey": [1, 2, 3],
            "objectKey": {
                "nestedKey": "nestedValue"
            },
            "nullKey": null
        }
        """;
        var model = new JsonDocumentModel();
        model.Deserialize(json);
        JsonDocumentNode[] expected = new[]
        {
            new JsonDocumentNode { Key = "textKey", Content = "\"text\"", UnderlyingNode = model.RootNode["textKey"] },
            new JsonDocumentNode { Key = "numberKey", Content = "123", UnderlyingNode = model.RootNode["numberKey"] },
            new JsonDocumentNode { Key = "booleanKey", Content = "true", UnderlyingNode = model.RootNode["booleanKey"] },
            new JsonDocumentNode { Key = "arrayKey", Content = "[1,2,3]", UnderlyingNode = model.RootNode["arrayKey"] },
            new JsonDocumentNode { Key = "objectKey", Content = "{\"nestedKey\":\"nestedValue\"}", UnderlyingNode = model.RootNode["objectKey"] },
            new JsonDocumentNode { Key = "nullKey", Content = "null", UnderlyingNode = model.RootNode["nullKey"] }
        };

        var nodes = model.GetNodes().ToArray();

        nodes.Should().BeEquivalentTo(expected);
    }

    // UpdateNodeContent

    [Fact]
    public void Should_update_node_content()
    {
        var json = """
        {
            "textKey": "text",
            "numberKey": 1,
            "booleanKey": false,
            "arrayKey": [1, 2, 3],
            "objectKey": {
                "nestedKey": "nestedValue"
            },
            "nullKey": "it will be null"
        }
        """;
        var expected = """
        {
            "textKey": "newText",
            "numberKey": 2,
            "booleanKey": false,
            "arrayKey": [4, 5],
            "objectKey": {
                "nestedKey": "new nestedValue"
            },
            "nullKey": null
        }
        """;
        var model = new JsonDocumentModel();
        model.Deserialize(json);
        var nodes = model.GetNodes();

        model.UpdateNodeContent(nodes.First(n => n.Key == "textKey"), "\"newText\"");
        model.UpdateNodeContent(nodes.First(n => n.Key == "numberKey"), "2");
        model.UpdateNodeContent(nodes.First(n => n.Key == "booleanKey"), "false");
        model.UpdateNodeContent(nodes.First(n => n.Key == "arrayKey"), "[4, 5]");
        model.UpdateNodeContent(nodes.First(n => n.Key == "objectKey"), "{\"nestedKey\":\"new nestedValue\"}");
        model.UpdateNodeContent(nodes.First(n => n.Key == "nullKey"), "null");

        var result = model.Serialize();
        result.Should().BeEquivalentTo(expected);
    }


}
