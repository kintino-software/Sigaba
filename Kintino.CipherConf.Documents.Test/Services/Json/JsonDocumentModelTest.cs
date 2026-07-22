//using System.Text.Json;

//namespace Kintino.CipherConf.Documents.Services.Json;

//public class JsonDocumentModelTest
//{
//    private readonly JsonDocumentModel model = new();

//    private void AssertJsonIsValid(string json)
//    {
//        using var doc = JsonDocument.Parse(
//            json,
//            new JsonDocumentOptions
//            {
//                AllowTrailingCommas = true,
//                CommentHandling = JsonCommentHandling.Skip
//            });
//    }

//    // Transform

//    [Fact]
//    public void Should_keep_original_content_and_formatting()
//    {
//        var originalJson = """
//        {
//            "textKey": "text",
//                            "numberKey": 123,
//            "booleanKey": true,
//            // comment 1
//            "arrayKey": [1, 2, 3],
//            "objectKey": {
//                    // comment 2
//                    "nestedKey": "nestedValue"
//            },
//            "nullKey": null
//        }
//        """;

//        var result = model.Transform(originalJson, n => n.RawContent);

//        result.Should().Be(originalJson);
//        AssertJsonIsValid(result);
//    }


//    [Theory]
//    [InlineData("newText")] // string (escaped, as it will be interpolated into the raw test json)
//    [InlineData("1")] // number
//    [InlineData("true")] // boolean
//    [InlineData("false")] // boolean
//    [InlineData("null")] // null
//    [InlineData("[1, 2, 3]")] // number array
//    [InlineData(@"[""a"", ""b"", ""c""]")] // string array
//    [InlineData(@"{""nestedKey"": ""nestedValue""}")] // object
//    public void Should_return_string_with_transformed_values(string newValue)
//    {
//        var json = """
//        {
//            // comment
//            "targetParent": {
//                "target": "original value"
//            }
//        }
//        """;
//        var expected = $$"""
//        {
//            // comment
//            "targetParent": {
//                "target": {{newValue}}
//            }
//        }
//        """;
//        var model = new JsonDocumentModel();

//        var result = model.Transform(json, n => n.Key == "targetParent.target" ? newValue : n.RawContent);

//        result.Should().Be(expected);
//        AssertJsonIsValid(result);
//    }

//    [Fact]
//    public void Should_pass_raw_value_to_transform_function()
//    {
//        var originalJson = """
//        {
//            "target": "value"
//        }
//        """;

//        int called = 0;
//        var result = model.Transform(originalJson, node =>
//        {
//            node.RawContent.Should().Be("\"value\"");
//            called++;
//            return node.RawContent;
//        });
//        called.Should().Be(1);
//    }

//    [Fact]
//    public void Should_throw_exception_when_deserializing_invalid_json()
//    {
//        var json = """
//        {
//            key: "value"
//        }
//        """;

//        var action = () => model.Transform(json, node => node.RawContent);

//        action.Should().Throw<JsonException>();
//    }

//}
