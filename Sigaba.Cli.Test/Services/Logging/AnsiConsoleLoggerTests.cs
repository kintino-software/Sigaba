using Microsoft.Extensions.Logging;
using Spectre.Console.Testing;

namespace Sigaba.Cli.Services.Logging;

public class AnsiConsoleLoggerTests
{
    private record Dummy(int Value);

    private readonly TestConsole console = new();

    private AnsiConsoleLogger CreateLogger(string name = "foo", LogLevel logLevel = LogLevel.Information) => new(name, logLevel, console);

    [Fact]
    public void Log_should_log()
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
    public void IsEnabled_should_match_the_log_level()
    {
        CreateLogger(logLevel: LogLevel.Warning).IsEnabled(LogLevel.Information).Should().BeFalse();
        CreateLogger(logLevel: LogLevel.Information).IsEnabled(LogLevel.Information).Should().BeTrue();
        CreateLogger(logLevel: LogLevel.Information).IsEnabled(LogLevel.Warning).Should().BeTrue();
    }
}

