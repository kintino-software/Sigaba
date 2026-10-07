using Sigaba.Documents.TestHelpers;

namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

public class KeyFsmTests
{
    private static KeyFsm CreateFsm(FsmContext ctx = null)
    {
        return new KeyFsm(ctx ?? FsmFaker.CrateFsmContext());
    }

    //

    [Fact]
    public void Should_handle_value()
    {
        var content = "foo=bar";
        var ctx = FsmFaker.CrateFsmContext(content: content);
        var fsm = CreateFsm(ctx);

        var result = fsm.Handle();

        result.Should().BeOfType<ValueFsm>();
        ctx.CurrToken.Should().NotBeNull();
        ctx.CurrToken.Key.Should().Be("foo");
        ctx.KeyBuffer.Length.Should().Be(0); // KeyBuffer should be cleared after processing
        ctx.Cursor.CurrIndex.Should().Be(3); // Cursor should be at the '=' character
    }

    [Theory]
    [InlineData("foo=bar")]
    [InlineData("f1oo=bar")]
    [InlineData("f__o=bar")]
    public void Should_accept_valid_key_names(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var action = () => fsm.Handle();

        action.Should().NotThrow<Exception>();
    }

    [Theory]
    // non exaustive list of invalid first characters for a key
    [InlineData("1foo=bar")]
    [InlineData("%foo=bar")]
    [InlineData("$foo=bar")]
    [InlineData("&foo=bar")]
    public void Handle_throw_with_invalid_first_char(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var action = () => fsm.Handle();

        action.Should().Throw<FormatException>("Invalid key name. Keys must start with a letter or an underscore (_), but found '*'.");
    }

    [Theory]
    // non exaustive list of invalid characters for a key
    [InlineData("f$oo=bar")]
    [InlineData("f#oo=bar")]
    [InlineData("f(oo=bar")]
    [InlineData("f\\oo=bar")]
    public void Handle_throw_with_invalid_char(string content)
    {
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var action = () => fsm.Handle();

        action.Should().Throw<FormatException>("Invalid key name. Keys should contain only letters, numbers or underscore (_), but found '*'.");
    }

    [Fact]
    public void Handle_throw_with_line_break_on_key_name()
    {
        var content = "foo\n=bar";
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var action = () => fsm.Handle();

        action.Should().Throw<FormatException>("Invalid key name. Keys should be single line, but found a line break.");
    }

    [Fact]
    public void Handle_throw_when_key_is_empty_name()
    {
        var content = "=bar";
        var ctx = FsmFaker.CrateFsmContext(content);
        var fsm = CreateFsm(ctx);

        var action = () => fsm.Handle();

        action.Should().Throw<FormatException>("Invalid key name. Keys should not be empty.");
    }

}

