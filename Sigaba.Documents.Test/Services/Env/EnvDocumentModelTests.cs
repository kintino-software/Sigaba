using Sigaba.Documents.Models;

namespace Sigaba.Documents.Services.Env;

public class EnvDocumentModelTests
{
    private IDocumentModel CreateDocument()
    {
        var model = new EnvDocumentModel();
        return model;
    }

    // Parse

    [Fact]
    public void Parse_should_parse_fields_correctly()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var model = CreateDocument();

        model.Parse(content);

        model.Should().NotBeNull();
        var fields = model.GetFieldNames().ToList();
        fields.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("                                   ")]
    public void Parse_should_parse_empty_document(string content)
    {
        var model = CreateDocument();

        model.Parse(content);

        model.Should().NotBeNull();
        var fields = model.GetFieldNames().ToList();
        fields.Should().BeEmpty();
    }

    // GetFieldNames

    [Fact]
    public void GetFieldNames_should_get_field_names()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var model = CreateDocument();

        model.Parse(content);

        model.GetFieldNames().Should().BeEquivalentTo(["key1", "key2", "key3"]);
    }

    // GetFieldRawValue

    [Fact]
    public void GetFieldRawValue_should_get_field_raw_value()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var model = CreateDocument();

        model.Parse(content);

        model.GetFieldRawValue("key1").Should().Be("value1");
        model.GetFieldRawValue("key2").Should().Be($$"""
            multi1 \
            multi2 \
            multi3
            """);
        model.GetFieldRawValue("key3").Should().Be("value3");
    }

    // GetFieldValue

    [Fact]
    public void GetFieldValue_should_get_field_value()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var model = CreateDocument();
        model.Parse(content);
        model.TryGetValue<string>("key1", out var value1).Should().BeTrue();
        value1.Should().Be("value1");
        model.TryGetValue<string>("key2", out var value2).Should().BeTrue();
        value2.Should().Be($$"""
            multi1 \
            multi2 \
            multi3
            """);
        model.TryGetValue<string>("key3", out var value3).Should().BeTrue();
        value3.Should().Be("value3");
    }

    [Fact]
    public void GetFieldValue_should_throw_when_type_is_not_string()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var model = CreateDocument();
        model.Parse(content);

        var action = () => model.TryGetValue<int>("key1", out var value);

        action.Should().Throw<NotSupportedException>().WithMessage("Type 'Int32' is not supported for env files.");
    }

    // Serialize

    [Fact]
    public void Serialize_should_return_original_content_if_not_changed()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var model = CreateDocument();
        model.Parse(content);

        var serialized = model.Serialize();

        serialized.Should().Be(content);
    }

    // misc

    [Fact]
    public void Should_replace_values()
    {
        var content = """
            key1=value1
            key2=multi1 \
            multi2 \
            multi3
            key3=value3
            """;
        var expected = """
            key1=newvalue1
            key2=newvalue2
            key3=value3
            """;
        var model = CreateDocument();
        model.Parse(content);

        model.SetFieldValue("key1", "newvalue1");
        model.SetFieldValue("key2", "newvalue2");
        var serialized = model.Serialize();

        serialized.Should().Be(expected);
    }
}

