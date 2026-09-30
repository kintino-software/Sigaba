using Sigaba.Documents.TestHelpers;

namespace Sigaba.Documents.Services.Env.Fsm.States;

public class CommentFsmTests
{
    private static CommentFsm CreateFsm(FsmContext ctx = null)
    {
        return new CommentFsm(ctx ?? FsmFaker.CrateFsmContext());
    }

    [Fact]
    public void Handle_should_handle_end_of_file()
    {
        var content = "# foobar";
        var ctx = FsmFaker.CrateFsmContext(content: content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeNull();
        ctx.Cursor.CurrIndex.Should().Be(7);
    }

    [Fact]
    public void Handle_should_next_line()
    {
        var content = "# foobar\nfoo=bar";
        var ctx = FsmFaker.CrateFsmContext(content: content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeOfType<LineStartFsm>();
        ctx.Cursor.CurrIndex.Should().Be(8);
    }
}

