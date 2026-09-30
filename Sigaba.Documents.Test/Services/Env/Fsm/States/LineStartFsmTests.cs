using Sigaba.Documents.TestHelpers;

namespace Sigaba.Documents.Services.Env.Fsm.States;

public class LineStartFsmTests
{
    private static LineStartFsm CreateFsm(FsmContext ctx = null)
    {
        return new LineStartFsm(ctx ?? FsmFaker.CrateFsmContext());
    }

    [Theory]
    [InlineData("foo=bar")]
    [InlineData("   foo=bar")]
    public void Should_detect_key(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().NotBeNull();
        result.Should().BeOfType<KeyFsm>();
    }

    [Theory]
    [InlineData("#comment")]
    [InlineData("#   comment")]
    [InlineData("  #  comment")]
    public void Should_detect_comment(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().NotBeNull();
        result.Should().BeOfType<CommentFsm>();
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("  \n")]
    public void Should_detect_new_line(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().NotBeNull();
        result.Should().BeOfType<LineStartFsm>();
    }

    [Theory]
    [InlineData("\\")]
    [InlineData("    $")]
    [InlineData("=")]
    public void Should_throw_when_invalid_char_is_detected(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var action = () => fsm.Handle();

        action.Should().Throw<FormatException>();
    }
}

