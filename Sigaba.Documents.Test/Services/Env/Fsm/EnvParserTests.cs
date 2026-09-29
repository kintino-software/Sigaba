namespace Sigaba.Documents.Services.Env.Fsm;

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

}

