using Microsoft.Extensions.Logging;
using Sigaba.Cli.Models;
using Spectre.Console;

namespace Sigaba.Cli.Services.Logging;

internal class AnsiConsoleLogger(IAnsiConsole console, IGlobalOptions globalOptions) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= globalOptions.Verbosity.ToLogLevel();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var color = logLevel switch
        {
            LogLevel.Trace => "grey",
            LogLevel.Debug => "grey",
            LogLevel.Information => "green",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            LogLevel.Critical => "red bold",
            _ => "white"
        };

        var message = formatter(state, exception);

        console.MarkupLine($"[{color}]{Markup.Escape(message)}[/]");

        if (exception is not null)
            console.WriteException(exception, ExceptionFormats.NoStackTrace);
    }
}
