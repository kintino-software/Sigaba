using Xunit.Abstractions;

namespace Sigaba.Documents.Services.Env.Interpreter;

public class EnvTokenizerTests(ITestOutputHelper output)
{
    [Fact]
    public void Test_full_document()
    {
        var content = """
            # first comment
            SIMPLE_KEY=simple_value
            # second comment
            KEY_WITH_SPACES=value with spaces
            KEY_WITH_COMMENT=value_with_comment # comment of the key
            KEY_WITH_CONTINUATION=key with continuation line 1 \
              key with continuation line 2 \
              key with continuation line 3
            KEY_WITH_QUOTES="key with quotes"
            """;

        //

        var result = EnvTokenizer.Tokenize(content).ToArray();

        foreach (var token in result)
            output.WriteLine(token.ToString());

        //

        result[0].Kind.Should().Be(TokenType.Comment);
        result[0].ToStringValue().Should().Be("# first comment");

        result[1].Kind.Should().Be(TokenType.Key);
        result[1].ToStringValue().Should().Be("SIMPLE_KEY");
        result[2].Kind.Should().Be(TokenType.Value);
        result[2].ToStringValue().Should().Be("simple_value");

        result[3].Kind.Should().Be(TokenType.Comment);
        result[3].ToStringValue().Should().Be("# second comment");

        result[4].Kind.Should().Be(TokenType.Key);
        result[4].ToStringValue().Should().Be("KEY_WITH_SPACES");
        result[5].Kind.Should().Be(TokenType.Value);
        result[5].ToStringValue().Should().Be("value with spaces");

        result[6].Kind.Should().Be(TokenType.Key);
        result[6].ToStringValue().Should().Be("KEY_WITH_COMMENT");
        result[7].Kind.Should().Be(TokenType.Value);
        result[7].ToStringValue().Should().Be("value_with_comment ");
        result[8].Kind.Should().Be(TokenType.Comment);
        result[8].ToStringValue().Should().Be("# comment of the key");

        result[9].Kind.Should().Be(TokenType.Key);
        result[9].ToStringValue().Should().Be("KEY_WITH_CONTINUATION");
        result[10].Kind.Should().Be(TokenType.Value);
        result[10].ToStringValue().Should().Be("key with continuation line 1 \\");
        result[11].Kind.Should().Be(TokenType.Value);
        result[11].ToStringValue().Should().Be("  key with continuation line 2 \\");
        result[12].Kind.Should().Be(TokenType.Value);
        result[12].ToStringValue().Should().Be("  key with continuation line 3");

        result[13].Kind.Should().Be(TokenType.Key);
        result[13].ToStringValue().Should().Be("KEY_WITH_QUOTES");
        result[14].Kind.Should().Be(TokenType.LiteralValue);
        result[14].ToStringValue().Should().Be("\"key with quotes\"");

    }
}

