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

        var message = formatter(state, exception);

        if (logLevel < LogLevel.Information)
            WriteDetailed(message, color, exception);
        else
            WriteNormal(message, color, exception);
    }

    private void WriteNormal(string message, string color, Exception? exception)
    {
        console.MarkupLine($"[{color}]{Markup.Escape(message)}[/]");

        if (exception is not null)
            console.WriteException(exception, ExceptionFormats.NoStackTrace);
    }

    private void WriteDetailed(string message, string color, Exception? exception)
    {
        // add the category
        console.MarkupLine($"[{color}][yellow]{categoryName}[/]: {Markup.Escape(message)}[/]");

        if (exception is not null)
            console.WriteException(exception, ExceptionFormats.ShortenTypes); // show the stack trace for detailed logs
    }
}
