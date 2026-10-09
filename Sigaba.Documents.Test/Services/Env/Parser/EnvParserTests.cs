namespace Sigaba.Documents.Services.Env.Parser;

public class EnvParserTests
{
    [Fact]
    public void Parse_should_do_something()
    {
        var content = """
            # This is a comment
            SIMPLE_KEY=simple_value
            KEY_WITH_COMMENT=key_with_comment # comment of the key
            KEY_WITH_SPACES=key with spaces
            KEY_WITH_CONTINUATION=line 1 \
            line 2 \
            line 3
            KEY_WITH_QUOTES="key with quotes"
            """.Replace("\r\n", "\n");

        var result = EnvParser.Parse(content);

        result.Should().ContainKeys(
            "SIMPLE_KEY",
            "KEY_WITH_COMMENT",
            "KEY_WITH_SPACES",
            "KEY_WITH_CONTINUATION",
            "KEY_WITH_QUOTES");

        result["SIMPLE_KEY"].Should().BeEquivalentTo(new EnvEntry("SIMPLE_KEY", "simple_value", 31, 12));

        result["KEY_WITH_COMMENT"].Should().BeEquivalentTo(new EnvEntry("KEY_WITH_COMMENT", "key_with_comment ", 61, 17));

        result["KEY_WITH_SPACES"].Should().BeEquivalentTo(new EnvEntry("KEY_WITH_SPACES", "key with spaces", 115, 15));

        result["KEY_WITH_CONTINUATION"].Should().BeEquivalentTo(new EnvEntry(
            "KEY_WITH_CONTINUATION",
            "line 1 \\\nline 2 \\\nline 3",
            153,
            24));


        result["KEY_WITH_QUOTES"].Should().BeEquivalentTo(new EnvEntry("KEY_WITH_QUOTES", "\"key with quotes\"", 194, 17));

    }
}

