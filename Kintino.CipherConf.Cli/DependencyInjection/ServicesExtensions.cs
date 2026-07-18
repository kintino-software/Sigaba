using Kintino.CipherConf.App.DependencyInjection;
using Kintino.CipherConf.Tooling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli.DependencyInjection;

internal static class ServicesExtensions
{
    public static void AddCli(this IServiceCollection services, IFileSystem fs, ITextEditor textEditor)
    {

        // in case any other assembly has already registered the following services, we want to override it with our own instance
        services.RemoveAll<IFileSystem>();
        services.RemoveAll<ITextEditor>();

        // solution services
        services.AddSingleton<ITextEditor>(textEditor);
        services.AddApp(new()
        {
            PrivateKeyFileName = "private.key",
            PublicKeyFileName = "public.key",
            ToolSettingsFileName = "ec.settings.json",
        });

        // 3rd party
        services.AddSingleton<IFileSystem>(fs);
        services.AddLogging(cfg => cfg.AddSimpleConsole(cfg =>
        {
            cfg.IncludeScopes = false;
            cfg.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
            cfg.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        }));
    }
}
