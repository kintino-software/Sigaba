using Kintino.CipherConf.Documents.Models;
using System.Text.Json;

namespace Kintino.CipherConf.Documents.Services.Json;

public class JsonDocumentModelTest
{
    private readonly IDocumentModel model = new JsonDocumentModel();

    private static void AssertJsonIsValid(string json)
    {
        using var doc = JsonDocument.Parse(
            json,
            new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });
    }

    // Parse

    [Fact]
    public void Should_keep_original_content_and_formatting()
    {
        var originalJson = """
        {
            "textKey": "text",
                            "numberKey": 123,
            "booleanKey": true,
            // comment 1
            "arrayKey": [1, 2, 3],
            "objectKey": {
                    // comment 2
                    "nestedKey": "nestedValue"
            },
            "nullKey": null
        }
        """;

        var action = () => model.Parse(originalJson);

        action.Should().NotThrow();

    }

    // GetFieldNames

    [Fact]
    public void Should_get_field_names()
    {
        var json = """
        {
            // comment
            "field1": "value1",
            "field2": 42,
            "targetParent": {
                "target": "original value"
            },
        }
        """;
        model.Parse(json);

        model.GetFieldNames().Should().BeEquivalentTo(["field1", "field2", "targetParent.target"]);
    }

    // SetFieldValue

    [Fact]
    public void Should_set_field_value()
    {
        var json = """
        {
            "field1": "value1",
            "field2": 42,
            "field3": true,
            "field4": [1, 2, 3],
            "targetParent": {
                "target": null
            }
        }
        """;
        var expected = """
        {
            "field1": 42,
            "field2": true,
            "field3": [1,2,3],
            "field4": null,
            "targetParent": {
                "target": "foobar"
            }
        }
        """;
        model.Parse(json);

        model.SetFieldValue("field1", 42);
        model.SetFieldValue("field2", true);
        model.SetFieldValue("field3", new int[] { 1, 2, 3 });
        model.SetFieldValue<object>("field4", null);
        model.SetFieldValue("targetParent.target", "foobar");
        var result = model.Serialize();

        AssertJsonIsValid(result);
        result.Should().Be(expected);
    }

    // SetFieldRawValue

    [Fact]
    public void Should_set_field_raw_value()
    {
        var json = """
        {
            "field1": "value1",
            "field2": 42,
            "field3": true,
            "field4": [1, 2, 3],
            "targetParent": {
                "target": null
            }
        }
        """;
        var expected = """
        {
            "field1": 42,
            "field2": true,
            "field3": [1,2,3],
            "field4": null,
            "targetParent": {
                "target": "foobar"
            }
        }
        """;
        model.Parse(json);

        model.SetFieldRawValue("field1", "42");
        model.SetFieldRawValue("field2", "true");
        model.SetFieldRawValue("field3", "[1,2,3]");
        model.SetFieldRawValue("field4", "null");
        model.SetFieldRawValue("targetParent.target", @"""foobar""");
        var result = model.Serialize();

        AssertJsonIsValid(result);
        result.Should().Be(expected);
    }

    // GetFieldRawValue

    [Fact]
    public void Should_get_field_raw_value()
    {
        var json = """
        {
            "field1": "value1",
            "field2": 42,
            "field3": true,
            "field4": [1, 2, 3],
            "targetParent": {
                "target": null
            }
        }
        """;
        model.Parse(json);

        model.GetFieldRawValue("field1").Should().Be(@"""value1""");
        model.GetFieldRawValue("field2").Should().Be("42");
        model.GetFieldRawValue("field3").Should().Be("true");
        model.GetFieldRawValue("field4").Should().Be("[1, 2, 3]");
        model.GetFieldRawValue("targetParent.target").Should().Be("null");
    }

    // GetFieldValue

    [Fact]
    public void Should_get_field_value()
    {
        var json = """
        {
            "field1": "value1",
            "field2": 42,
            "field3": true,
            "field4": [1, 2, 3],
            "targetParent": {
                "target": null
            }
        }
        """;
        model.Parse(json);

        model.TryGetValue<string>("field1", out var field1).Should().BeTrue();
        field1.Should().Be("value1");
        model.TryGetValue<int>("field2", out var field2).Should().BeTrue();
        field2.Should().Be(42);
        model.TryGetValue<bool>("field3", out var field3).Should().BeTrue();
        field3.Should().Be(true);
        model.TryGetValue<int[]>("field4", out var field4).Should().BeTrue();
        field4.Should().BeEquivalentTo(new int[] { 1, 2, 3 });
        model.TryGetValue<object>("targetParent.target", out var target).Should().BeTrue();
        target.Should().BeNull();
    }

    // Serialize

    [Fact]
    public void Should_serialize_document_keeping_format()
    {
        var json = """
            {
            // comment 1
            "field1": "value1",
                        "targetParent": {
                "target": null
                // comment 2
            }
                            }
        """;

        var expected = """
            {
            // comment 1
            "field1": "foobar",
                        "targetParent": {
                "target": null
                // comment 2
            }
                            }
        """;
        model.Parse(json);
        model.SetFieldRawValue("field1", @"""foobar""");

        var result = model.Serialize();

        AssertJsonIsValid(result);
        result.Should().Be(expected);
    }
}
