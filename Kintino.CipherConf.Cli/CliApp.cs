using Kintino.CipherConf.App;
using Kintino.CipherConf.App.DependencyInjection;
using Kintino.CipherConf.Cli.Adaptors.SpectreConsole;
using Kintino.CipherConf.Cli.Commands;
using Kintino.CipherConf.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli;

/// <summary>
/// Wrapper class for the console application that sets up dependency injection and runs the application with the provided arguments.
/// <br/>
/// This wrapper serves as anti-corruption layer to prevent the Spectre.Console library from leaking into the rest of the application.
/// </summary>
public class CliApp
{
    public CommandApp CommandApp { get; }

    public CliApp(IFileSystem? fs = null)
    {
        // services

        CommandApp = SpectreConsoleHelper.CreateCommandApp(services =>
        {
            services.AddApp(fs);
            services.AddSingleton<ITextEditor>(new WindowsEditTextEditor());
            // 3rd party
            services.AddLogging(cfg => cfg.AddSimpleConsole(cfg =>
            {
                cfg.IncludeScopes = false;
                cfg.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                cfg.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
            }));

        });

        // cli config

        CommandApp.Configure(config =>
        {
            config.PropagateExceptions();
            config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
            config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts the specified configuration.");
            config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts the specified configuration.");
            config.AddCommand<EditCommand>("edit").WithDescription("Edits the specified configuration.");
        });
    }

    public Task<int> RunAsync(params string[] args)
    {
        return CommandApp.RunAsync(args);
    }
}
