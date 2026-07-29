namespace Kintino.CipherConf.Documents.Services.Json;

public class RawFieldTest
{
    // instantiation

    [Theory]
    [InlineData("k", "\"v\"")]
    [InlineData("key", "1")]
    [InlineData("key", "true")]
    [InlineData("key", "false")]
    [InlineData("key", "null")]
    [InlineData("key", "{ \"a\": 1 }")]
    [InlineData("key", "[1, 2, 3]")]
    [InlineData("key", "[\"a\", \"b\"]")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "\"vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv\"")]
    public void Should_instantiate_with_valid_json(string key, string value)
    {
        var metaField = new RawField(key, value);

        metaField.Key.Should().Be(key);
        metaField.RawValue.Should().Be(value);
    }

    [Theory]
    [InlineData("", "\"v\"")]
    [InlineData(" ", "\"v\"")]
    [InlineData(null, "\"v\"")]
    public void Should_throw_if_key_is_invalid(string key, string value)
    {
        var action = () => new RawField(key, value);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid json key*");
    }

    [Theory]
    [InlineData("key", "")]
    [InlineData("key", " ")]
    [InlineData("key", null)]
    [InlineData("key", "invalid json")]
    [InlineData("key", "{\"malformed : 1 }")]
    public void Should_throw_if_raw_value_is_invalid(string key, string value)
    {
        var action = () => new RawField(key, value);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid JSON value for key '*'*");

    }

    // SetRawValue

    [Theory]
    [InlineData("\"v\"")]
    [InlineData("1")]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData("null")]
    [InlineData("{ \"a\": 1 }")]
    [InlineData("[1, 2, 3]")]
    [InlineData("[\"a\", \"b\"]")]
    [InlineData("\"vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv\"")]
    public void Should_set_raw_value(string value)
    {
        var metaField = new RawField("key", "1");
        metaField.SetRawValue(value);
        metaField.RawValue.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid json")]
    [InlineData("{\"malformed : 1 }")]
    public void Should_throw_when_setting_raw_invalid_raw_value(string value)
    {
        var metaField = new RawField("key", "1");
        var action = () => metaField.SetRawValue(value);
        action.Should().Throw<ArgumentException>().WithMessage("Invalid JSON value for key '*'*");
    }

    // SetValue

    [Fact]
    public void Should_set_value()
    {
        var values = new List<object>
        {
            "string",
            1,
            true,
            false,
            null,
            new { a = 1, b = "test" },
            new List<int> { 1, 2, 3 }
        };

        foreach (var value in values)
        {
            var metaField = new RawField("key", "1");
            metaField.SetValue(value);
            var expectedRawValue = System.Text.Json.JsonSerializer.Serialize(value);
            metaField.RawValue.Should().Be(expectedRawValue);
        }
    }

    // TryGetValue

    [Fact]
    public void Should_try_get_value()
    {
        var metaField = new RawField("key", "\"string\"");

        metaField.TryGetValue<string>(out var stringValue).Should().BeTrue();
        stringValue.Should().Be("string");

        metaField.SetRawValue("1");
        metaField.TryGetValue<int>(out var intValue).Should().BeTrue();
        intValue.Should().Be(1);

        metaField.SetRawValue("true");
        metaField.TryGetValue<bool>(out var boolValue).Should().BeTrue();
        boolValue.Should().BeTrue();

        metaField.SetRawValue("{ \"a\": 1 }");
        metaField.TryGetValue<Dictionary<string, int>>(out var dictValue).Should().BeTrue();
        dictValue.Should().ContainSingle("a").And.Subject["a"].Should().Be(1);

        metaField.SetRawValue("[1, 2, 3]");
        metaField.TryGetValue<List<int>>(out var listValue).Should().BeTrue();
        listValue.Should().BeEquivalentTo([1, 2, 3]);
    }

}

