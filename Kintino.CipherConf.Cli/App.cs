using Kintino.CipherConf.Cli.Adaptors.SpectreConsole;
using Kintino.CipherConf.Cli.Commands;
using Kintino.CipherConf.Cli.DependencyInjection;
using Kintino.CipherConf.Tooling;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli;

/// <summary>
/// Wrapper class for the console application that sets up dependency injection and runs the application with the provided arguments.
/// <br/>
/// This wrapper serves as anti-corruption layer to prevent the Spectre.Console library from leaking into the rest of the application.
/// </summary>
public class App
{
    public CommandApp CommandApp { get; }

    public App(IFileSystem fs, ITextEditor textEditor)
    {
        CommandApp = SpectreConsoleHelper.CreateCommandApp(services =>
        {
            services.AddECCli(fs, textEditor);
        });
        CommandApp.Configure(config =>
        {
            config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
            config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts the specified configuration.");
            config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts the specified configuration.");
            config.AddCommand<EditCommand>("edit").WithDescription("Edits the specified configuration.");
        });
    }

    public async Task RunAsync(params string[] args)
    {
        var exitCode = await CommandApp.RunAsync(args);
        if (exitCode != 0)
            throw new ApplicationException("Application exited with a non-zero exit code: " + exitCode);

    }
}
