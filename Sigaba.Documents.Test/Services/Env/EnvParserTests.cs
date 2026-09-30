using Sigaba.Documents.Services.Env.Fsm;

namespace Sigaba.Documents.Services.Env;

public class EnvParserTests
{
    private EnvParser CreateParser()
    {
        var parser = new EnvParser();
        return parser;
    }

    [Fact]
    public void Should_parse_simple_content()
    {
        //idx          01234567890 123456789012 34567890123
        var content = "key1=value1\nkey2=value2\nkey3=value3";
        var parser = CreateParser();

        var result = parser.Parse(content);

        result.Should().BeEquivalentTo(new Dictionary<string, EnvEntry>()
        {
            ["key1"] = new EnvEntry("key1", 5, 6),
            ["key2"] = new EnvEntry("key2", 17, 6),
            ["key3"] = new EnvEntry("key3", 29, 6),
        });
    }

    [Fact]
    public void Should_parse_continuation_values()
    {
        //idx          01234567890 123456789012 3 4567890 1 234567 890123456789
        var content = "key1=value1\nkey2=multi1\\\nmulti2\\\nmulti3\nkey3=value3";
        var parser = CreateParser();

        var result = parser.Parse(content);

        result.Should().BeEquivalentTo(new Dictionary<string, EnvEntry>()
        {
            ["key1"] = new EnvEntry("key1", 5, 6),
            ["key2"] = new EnvEntry("key2", 17, 22),
            ["key3"] = new EnvEntry("key3", 45, 6),
        });
    }

    [Fact]
    public void Should_parse_quoted_values()
    {
        //                       1           2           3
        //idx          01234567890 123456 7890123 4 567890123456
        var content = "key1=value1\nkey2=\"foobar\"\nkey3=value3";
        var parser = CreateParser();

        var result = parser.Parse(content);

        result.Should().BeEquivalentTo(new Dictionary<string, EnvEntry>()
        {
            ["key1"] = new EnvEntry("key1", 5, 6),
            ["key2"] = new EnvEntry("key2", 17, 8),
            ["key3"] = new EnvEntry("key3", 31, 6),
        });
    }

    [Theory]
    [InlineData("FOO\nBAR=1", "ENV001")]
    [InlineData("1BAD=1", "ENV003")]
    [InlineData("A=\"unclosed", "ENV004")]
    [InlineData("A=1\\", "ENV005")]  // dangling at EOF
    [InlineData("A=1 \\", "ENV005")]  // ws before backslash
    [InlineData("A=1\\ # c", "ENV005")]  // comment after backslash
    public void Should_throw_parse_exceptions(string input, string code)
    {
        var parser = CreateParser();

        var action = () => parser.Parse(input);

        action.Should().ThrowExactly<EnvParseException>();

    }

}

