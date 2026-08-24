using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.App.DependencyInjection;
using Sigaba.Cli.Models;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Cli.Services.Logging;
using Sigaba.Cli.Services.TextEditors;
using Spectre.Console;

namespace Sigaba.Cli.DependencyInjection;

internal static class ServicesExtensions
{
    public static IServiceCollection AddCliApp(this IServiceCollection services, IGlobalOptions globalOptions)
    {
        services
            .AddApp()
            .AddLogging(builder =>
            {
                builder.AddAnsiConsole();
                builder.SetMinimumLevel(globalOptions.Verbosity.ToLogLevel());
            })
            .AddSingleton<IAnsiConsole>(AnsiConsole.Console)
            .AddSingleton<IGlobalOptions>(globalOptions)
            .AddSingleton<ITextEditor, TextEditor>()
            .AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console)
            .AddSingleton<CliStopWatch>();

        return services;
    }
}
