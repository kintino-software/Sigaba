using Sigaba.Documents.TestHelpers;

namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

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
            ctx.CurrToken = new EnvToken { Key = key };

        }
        return ctx;
    }

    //

    [Theory]
    [InlineData("bar", "bar", 0, 2)]
    [InlineData("  bar", "bar", 0, 4)]
    [InlineData("bar  ", "bar  ", 0, 4)]
    [InlineData("bar  \n  ", "bar  ", 0, 4)]
    [InlineData("\tbar\n", "bar", 0, 2)]
    [InlineData("\tbar\t", "bar\t", 0, 2)]
    public void Should_handle_plain_values(
        string content,
        string expectedValue,
        int expectedValueStartIndex,
        int expectedValueEndIndex)
    {
        var ctx = CreateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeOfType<LineStartFsm>();
        ctx.Tokens[0].Should().BeEquivalentTo(
            new EnvToken
            {
                Key = "key",
                Value = expectedValue,
                RawValueStartIndex = expectedValueStartIndex,
                RawValueEndIndex = expectedValueEndIndex
            }
        );
        ctx.CurrToken.Should().BeNull();
    }

    [Theory]
    [InlineData("abc#def", 0, 6)]
    [InlineData("  bar#foo", 0, 8)]
    [InlineData("bar  #foo", 0, 8)]
    public void Should_handle_plain_values_with_comments(
        string content,
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
                RawValueStartIndex = expectedValueStartIndex,
                RawValueEndIndex = expectedValueEndIndex
            }
        );
        ctx.CurrToken.Should().BeNull();
    }

    [Theory]
    [InlineData("\"bar\"", 0, 4)]
    [InlineData("  \"bar\"", 0, 6)]
    [InlineData("\"bar\"  ", 0, 6)]
    public void Should_handle_quoted_values(
        string content,
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
                RawValueStartIndex = expectedValueStartIndex,
                RawValueEndIndex = expectedValueEndIndex
            }
        );
        ctx.CurrToken.Should().BeNull();
    }
}

