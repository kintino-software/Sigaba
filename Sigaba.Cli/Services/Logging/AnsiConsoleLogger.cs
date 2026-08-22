using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Sigaba.Cli.Services.Logging;

internal class AnsiConsoleLogger(string categoryName, LogLevel minLevel, IAnsiConsole console) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= minLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);

        var color = logLevel switch
        {
            LogLevel.Trace => "grey",
            LogLevel.Debug => "grey",
            LogLevel.Information => "white",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            LogLevel.Critical => "red bold",
            _ => "white"
        };

        console.MarkupLine($"[{color}]{Markup.Escape(message)}[/]");

        if (exception is not null)
            console.WriteException(exception, ExceptionFormats.ShortenPaths);
    }
}
