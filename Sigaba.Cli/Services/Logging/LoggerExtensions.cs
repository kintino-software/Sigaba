using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Sigaba.Cli.Services.Logging;

internal static class LoggerExtensions
{
    public static ILoggingBuilder AddAnsiConsole(this ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AnsiConsoleLoggerProvider>(
            sp => new AnsiConsoleLoggerProvider(sp.GetRequiredService<IAnsiConsole>())));

        return builder;
    }
}
