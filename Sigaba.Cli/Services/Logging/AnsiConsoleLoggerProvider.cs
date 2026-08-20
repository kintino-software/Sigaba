using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Collections.Concurrent;

namespace Sigaba.Cli.Services.Logging;

[ProviderAlias("AnsiConsole")]
public sealed class AnsiConsoleLoggerProvider(IAnsiConsole console) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, AnsiConsoleLogger> loggers = [];

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(
            categoryName,
            name => new AnsiConsoleLogger(name, console));
    }

    public void Dispose()
    {
        loggers.Clear();
    }
}
