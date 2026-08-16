using Sigaba.Documents.Models;
using System.Text.Json;

namespace Sigaba.Documents.Services.Json;

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
  public void Should_parse_content_with_comments_and_trailing_commas()
  {
    var originalJson = """
        // comment
        {
            // comment
            "textKey": "text",
            "numberKey": 123,
            "booleanKey": true,
            "arrayKey": [1, 2, 3],
            "objectKey": {
                "nestedKey": "nestedValue", // trailing comma
            },
            "nullKey": null, // trailing comma
        }
        """;
    AssertJsonIsValid(originalJson);

    var action = () => model.Parse(originalJson);

    action.Should().NotThrow();
  }

  // Serialize

  [Fact]
  public void Should_serialize_content()
  {
    var originalJson = """
        // comment
        {
            // comment
            "textKey": "text",
            "numberKey": 123,
            "booleanKey": true,
            "arrayKey": [1, 2, 3],
            "objectKey": {
                "nestedKey": "nestedValue", // trailing comma
            },
            "nullKey": null, // trailing comma
        }
        """;
    model.Parse(originalJson);

    var actualJson = model.Serialize();

    actualJson.Should().Be(originalJson);
    AssertJsonIsValid(actualJson);
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
            "field5": null,
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
    field4.Should().BeEquivalentTo([1, 2, 3]);
    model.TryGetValue<object>("targetParent.target", out var target).Should().BeTrue();
    target.Should().BeNull();
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
            "field3": [1, 2, 3],
            "field4": null,
            "targetParent": {
                "target": "foobar"
            }
        }
        """;
    model.Parse(json);

    model.SetFieldRawValue("field1", "42");
    model.SetFieldRawValue("field2", "true");
    model.SetFieldRawValue("field3", "[1, 2, 3]");
    model.SetFieldRawValue("field4", "null");
    model.SetFieldRawValue("targetParent.target", @"""foobar""");
    var result = model.Serialize();

    AssertJsonIsValid(result);
    result.Should().Be(expected);
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
}
