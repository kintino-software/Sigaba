namespace Sigaba.Documents.Services.Env.Fsm;

public class EnvParserTests
{
    private EnvParser CreateParser()
    {
        var parser = new EnvParser();
        return parser;
    }

    [Fact]
    public void Should_parse()
    {
        //             012345678901234567890123
        var content = "key1=value1\nkey2=value2";
        var parser = CreateParser();

        var result = parser.Parse(content);

        result.Should().BeEquivalentTo(new Dictionary<string, EnvEntry>()
        {
            ["key1"] = new EnvEntry("key1", 5, 6),
            ["key2"] = new EnvEntry("key2", 8, 6),
        });
    }

}

