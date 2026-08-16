using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Cli.Services.TextEditors;
using Spectre.Console;

namespace Sigaba.Cli.DependencyInjection;

internal static class ServicesExtensions
{
    public static IServiceCollection AddCliApp(this IServiceCollection services)
    {
        services
            .AddSingleton<ITextEditor, TextEditor>()
            .AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console)
            .AddSingleton<CliStopWatch>()
            .AddLogging(cfg => cfg.AddSimpleConsole(cfg =>
                {
                    cfg.IncludeScopes = false;
                    cfg.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                    cfg.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                }));

        return services;
    }
}
