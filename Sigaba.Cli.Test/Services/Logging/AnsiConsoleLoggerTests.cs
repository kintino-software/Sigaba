using Microsoft.Extensions.Logging;
using Spectre.Console.Testing;

namespace Sigaba.Cli.Services.Logging;

public class AnsiConsoleLoggerTests
{
    private record Dummy(int Value);

    private readonly TestConsole console = new();

    private AnsiConsoleLogger CreateLogger() => new(console);

    [Fact]
    public void Should_log()
    {
        var logger = CreateLogger();

        Exception exceptionFormatterArg = null;
        logger.Log<Dummy>(
            logLevel: LogLevel.Information,
            eventId: 0,
            state: new Dummy(42),
            exception: null,
            formatter: (state, ex) =>
            {
                exceptionFormatterArg = ex;
                return state.Value.ToString();
            });

        console.Output.Should().Be("42\n"); // then end line is added because we call WriteLine in the logger implementation
        exceptionFormatterArg.Should().BeNull();
    }

    [Fact]
    public void Should_enable_log_levels_correctly()
    {
        CreateLogger().IsEnabled(LogLevel.Trace).Should().BeTrue();
        CreateLogger().IsEnabled(LogLevel.Debug).Should().BeTrue();
        CreateLogger().IsEnabled(LogLevel.Information).Should().BeTrue();
        CreateLogger().IsEnabled(LogLevel.Warning).Should().BeTrue();
        CreateLogger().IsEnabled(LogLevel.Error).Should().BeTrue();
        CreateLogger().IsEnabled(LogLevel.Critical).Should().BeTrue();
        CreateLogger().IsEnabled(LogLevel.None).Should().BeFalse();
    }
}

