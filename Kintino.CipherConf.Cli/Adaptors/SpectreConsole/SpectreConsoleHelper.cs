using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Kintino.CipherConf.Cli.Adaptors.SpectreConsole;

internal static class SpectreConsoleHelper
{
    public static CommandApp CreateCommandApp(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);
        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);
        return app;
    }

}
