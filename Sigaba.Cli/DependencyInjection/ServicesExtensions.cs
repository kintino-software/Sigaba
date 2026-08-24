using Microsoft.Extensions.DependencyInjection;
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
    public static IServiceCollection AddCliApp(this IServiceCollection services)
    {
        services.AddApp();

        services
            .AddSingleton<IGlobalOptions, GlobalOptions>()
            .AddSingleton<ITextEditor, TextEditor>()
            .AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console)
            .AddSingleton<CliStopWatch>();

        services.AddLogging(builder =>
        {
            builder.AddAnsiConsole();
        });

        return services;
    }
}
