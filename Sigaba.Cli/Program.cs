using Spectre.Console.Cli;

namespace Sigaba.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        var commandApp = new CommandApp(CommandAppSetup.CreateTypeRegistrar());
        commandApp.Configure(cfg => CommandAppSetup.Configurator(cfg, additionalConfig: null));
        await commandApp.RunAsync(args);
    }
}
