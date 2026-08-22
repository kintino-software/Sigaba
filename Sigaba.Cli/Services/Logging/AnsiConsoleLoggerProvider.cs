using Microsoft.Extensions.Logging;
using Sigaba.Cli.Models;
using Spectre.Console;
using System.Collections.Concurrent;

namespace Sigaba.Cli.Services.Logging;

[ProviderAlias("AnsiConsole")]
internal sealed class AnsiConsoleLoggerProvider(IAnsiConsole console, IGlobalOptions globalOptions) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, AnsiConsoleLogger> loggers = [];

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(
            categoryName,
            name => new AnsiConsoleLogger(name, globalOptions.Verbosity.ToLogLevel(), console));
    }

    public void Dispose()
    {
        loggers.Clear();
    }


}
