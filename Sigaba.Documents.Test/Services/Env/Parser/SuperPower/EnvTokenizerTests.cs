using Xunit.Abstractions;
using Sigaba.Documents.Services.Env.Parser;
using Sigaba.Documents.Services.Env.Parser.Superpower;

namespace Sigaba.Documents.Services.Env.Parser.SuperPower;

public class EnvTokenizerTests(ITestOutputHelper output)
{
    [Fact]
    public void Test_full_document()
    {
        var content = """
            # first comment
            SIMPLE_VALUE=simple_value
            # second comment
            VALUE_WITH_SPACES=value with spaces
            VALUE_WITH_COMMENT=value_with_comment # comment of the key
            VALUE_WITH_CONTINUATION=key with continuation line 1 \
              key with continuation line 2 \
              key with continuation line 3
            VALUE_WITH_QUOTES="key with quotes"
            VALUE_WITH_QUOTES_MULTI_LINE="line 1
            line 2
              line 3"
            VALUE_WITH_ESCAPED_QUOTES='key with \n\n\n escaped quotes'
            VALUE_WITH_ESCAPED_QUOTES_MULTI_LINE='
            line 1
            line 2
              line 3'
            """;

        //

        var result = EnvTokenizer.Tokenize(content).ToArray();

        foreach (var token in result)
            output.WriteLine(token.ToString());

        //

        result[0].Kind.Should().Be(TokenType.Comment);
        result[0].ToStringValue().Should().Be("# first comment");

        result[1].Kind.Should().Be(TokenType.Key);
        result[1].ToStringValue().Should().Be("SIMPLE_VALUE");
        result[2].Kind.Should().Be(TokenType.Value);
        result[2].ToStringValue().Should().Be("simple_value");

        result[3].Kind.Should().Be(TokenType.Comment);
        result[3].ToStringValue().Should().Be("# second comment");

        result[4].Kind.Should().Be(TokenType.Key);
        result[4].ToStringValue().Should().Be("VALUE_WITH_SPACES");
        result[5].Kind.Should().Be(TokenType.Value);
        result[5].ToStringValue().Should().Be("value with spaces");

        result[6].Kind.Should().Be(TokenType.Key);
        result[6].ToStringValue().Should().Be("VALUE_WITH_COMMENT");
        result[7].Kind.Should().Be(TokenType.Value);
        result[7].ToStringValue().Should().Be("value_with_comment ");
        result[8].Kind.Should().Be(TokenType.Comment);
        result[8].ToStringValue().Should().Be("# comment of the key");

        result[9].Kind.Should().Be(TokenType.Key);
        result[9].ToStringValue().Should().Be("VALUE_WITH_CONTINUATION");
        result[10].Kind.Should().Be(TokenType.MultiLineValue);
        result[10].ToStringValue().Should().Be("""
            key with continuation line 1 \
              key with continuation line 2 \
              key with continuation line 3
            """);

        result[11].Kind.Should().Be(TokenType.Key);
        result[11].ToStringValue().Should().Be("VALUE_WITH_QUOTES");
        result[12].Kind.Should().Be(TokenType.LiteralValue);
        result[12].ToStringValue().Should().Be("\"key with quotes\"");

        result[13].Kind.Should().Be(TokenType.Key);
        result[13].ToStringValue().Should().Be("VALUE_WITH_QUOTES_MULTI_LINE");
        result[14].Kind.Should().Be(TokenType.LiteralValue);
        result[14].ToStringValue().Should().Be("""
            "line 1
            line 2
              line 3"
            """);

        result[15].Kind.Should().Be(TokenType.Key);
        result[15].ToStringValue().Should().Be("VALUE_WITH_ESCAPED_QUOTES");
        result[16].Kind.Should().Be(TokenType.EscapedLiteralValue);
        result[16].ToStringValue().Should().Be("'key with \\n\\n\\n escaped quotes'");

        result[17].Kind.Should().Be(TokenType.Key);
        result[17].ToStringValue().Should().Be("VALUE_WITH_ESCAPED_QUOTES_MULTI_LINE");
        result[18].Kind.Should().Be(TokenType.EscapedLiteralValue);
        result[18].ToStringValue().Should().Be("""
            '
            line 1
            line 2
              line 3'
            """);
    }
}

