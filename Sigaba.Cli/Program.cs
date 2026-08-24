using Spectre.Console.Cli;

namespace Sigaba.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        var setup = AnsiConsoleSetup.Create();
        var commandApp = new CommandApp(setup.TypeRegistrar);
        commandApp.Configure(setup.Configurator);

        await commandApp.RunAsync(args);
    }
}
