using Spectre.Console.Cli;

namespace Sigaba.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        var commandApp = new CommandApp(AnsiConsoleSetup.CreateTypeRegistrar());
        commandApp.Configure(cfg => AnsiConsoleSetup.Configurator(cfg, additionalConfig: null));
        await commandApp.RunAsync(args);
    }
}
