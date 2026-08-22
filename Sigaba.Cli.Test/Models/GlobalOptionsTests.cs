namespace Sigaba.Cli.Models;

public class GlobalOptionsTests
{
    [Theory]
    [InlineData("-v normal a b c d", "a b c d")]
    [InlineData("a b -v normal c d", "a b c d")]
    [InlineData("a b c d -v normal", "a b c d")]
    public void Should_create_from_args(string argsStr, string expectedRemainingArgsStr)
    {
        var args = argsStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var globalOptions = GlobalOptions.ParseFromArgs(args, out var remainingArgs);

        globalOptions.Verbosity.Should().Be(VerbosityLevel.Normal);
        remainingArgs.Should().BeEquivalentTo(expectedRemainingArgsStr.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    [Theory]
    [InlineData("-v detailed")]
    [InlineData("--verbosity detailed")]
    public void Should_parse_verbosity(string argsStr)
    {
        var args = argsStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var globalOptions = GlobalOptions.ParseFromArgs(args, out var remainingArgs);

        globalOptions.Verbosity.Should().Be(VerbosityLevel.Detailed);
        remainingArgs.Should().BeEmpty();
    }

    [Theory]
    [InlineData("-q")]
    [InlineData("--quiet")]
    public void Should_set_quiet_mode(string args)
    {
        var globalOptions = GlobalOptions.ParseFromArgs([args], out var remainingArgs);

        globalOptions.Verbosity.Should().Be(VerbosityLevel.Quiet);
        remainingArgs.Should().BeEmpty();
    }
}

