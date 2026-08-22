using Sigaba.Cli.Models;
using Spectre.Console.Cli;

namespace Sigaba.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        var globalOptions = GlobalOptions.ParseFromArgs(args, out var remainingArgs);

        var commandApp = new CommandApp(AnsiConsoleSetup.CreateTypeRegistrar(globalOptions));
        commandApp.Configure(cfg => AnsiConsoleSetup.Configure(cfg, additionalConfig: null));

        await commandApp.RunAsync(remainingArgs);
    }
}
