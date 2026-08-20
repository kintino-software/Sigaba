using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Sigaba.Cli.Services.Logging;

internal static class LoggerExtensions
{
    public static ILoggingBuilder AddAnsiConsole(this ILoggingBuilder builder, IAnsiConsole? console = null)
    {
        builder.Services.TryAddSingleton<IAnsiConsole>(console ?? AnsiConsole.Console);
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, AnsiConsoleLoggerProvider>());

        return builder;
    }
}
