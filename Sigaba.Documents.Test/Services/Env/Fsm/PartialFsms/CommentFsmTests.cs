using Sigaba.Documents.TestHelpers;

namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

public class CommentFsmTests
{
    private static CommentFsm CreateFsm(FsmContext ctx = null)
    {
        return new CommentFsm(ctx ?? FsmFaker.CrateFsmContext());
    }

    [Theory]
    [InlineData("foobar", 5)]
    public void Should_handle_end_of_file(string content, int expectedFinalIndex)
    {
        var ctx = FsmFaker.CrateFsmContext(content: content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeNull();
        ctx.Cursor.CurrIndex.Should().Be(expectedFinalIndex);
    }

    [Theory]
    [InlineData(" foobar\nfoo=bar", 7)] // The cursor should consume the new line character
    [InlineData("foobar\n", 6)]
    public void Handle_should_next_line(string content, int expectedFinalIndex)
    {
        var ctx = FsmFaker.CrateFsmContext(content: content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeOfType<LineStartFsm>();
        ctx.Cursor.CurrIndex.Should().Be(expectedFinalIndex);
    }
}

