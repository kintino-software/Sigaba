using Sigaba.Documents.TestHelpers;

namespace Sigaba.Documents.Services.Env.Fsm.States;

public class ValueFsmTests
{
    private static ValueFsm CreateFsm(FsmContext ctx = null)
    {
        return new ValueFsm(ctx ?? FsmFaker.CrateFsmContext());
    }

    private static FsmContext CreateFsmContext(string content, string key = "key")
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        if (key != null)
        {
            ctx.CurrToken = new EnvToken { Key = key, ParsedValue = null };

        }
        return ctx;
    }

    //

    [Theory]
    [InlineData("bar", "bar", "bar", 0, 2)]
    [InlineData("  bar", "bar", "  bar", 0, 4)]
    [InlineData("bar  ", "bar  ", "bar  ", 0, 4)]
    [InlineData("bar  \n  ", "bar  ", "bar  ", 0, 4)]
    [InlineData("bar\n", "bar", "bar", 0, 2)]
    public void Should_handle_plain_values(
        string content,
        string expectedParsedValue,
        string expectedRawValue,
        int expectedValueStartIndex,
        int expectedValueEndIndex)
    {
        var ctx = CreateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        ctx.Tokens[0].Should().BeEquivalentTo(
            new EnvToken
            {
                Key = "key",
                ParsedValue = expectedParsedValue,
                RawValue = expectedRawValue,
                RawValueStartIndex = expectedValueStartIndex,
                RawValueEndIndex = expectedValueEndIndex
            }
        );
        ctx.CurrToken.Should().BeNull();
    }

    [Theory]
    [InlineData("abc#def", "abc", "abc#def", 0, 6)]
    [InlineData("  bar#foo", "bar", "  bar#foo", 0, 8)]
    [InlineData("bar  #foo", "bar  ", "bar  #foo", 0, 8)]
    public void Should_handle_plain_values_with_comments(
        string content,
        string expectedParsedValue,
        string expectedRawValue,
        int expectedValueStartIndex,
        int expectedValueEndIndex)
    {
        var ctx = CreateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        ctx.Tokens[0].Should().BeEquivalentTo(
            new EnvToken
            {
                Key = "key",
                ParsedValue = expectedParsedValue,
                RawValue = expectedRawValue,
                RawValueStartIndex = expectedValueStartIndex,
                RawValueEndIndex = expectedValueEndIndex
            }
        );
        ctx.CurrToken.Should().BeNull();
    }

    [Theory]
    [InlineData("\"bar\"", "bar", "\"bar\"", 0, 4)]
    [InlineData("  \"bar\"", "bar", "  \"bar\"", 0, 6)]
    [InlineData("\"bar\"  ", "bar", "\"bar\"  ", 0, 6)]
    public void Should_handle_quoted_values(
        string content,
        string expectedParsedValue,
        string expectedRawValue,
        int expectedValueStartIndex,
        int expectedValueEndIndex)
    {
        var ctx = CreateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeNull();
        ctx.Tokens[0].Should().BeEquivalentTo(
            new EnvToken
            {
                Key = "key",
                ParsedValue = expectedParsedValue,
                RawValue = expectedRawValue,
                RawValueStartIndex = expectedValueStartIndex,
                RawValueEndIndex = expectedValueEndIndex
            }
        );
        ctx.CurrToken.Should().BeNull();
    }
}

