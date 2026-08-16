using Sigaba.App;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Primitives.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Init;


internal class InitCommand(ISigabaApp app, IFileSystem fs, IAnsiConsole console, CliStopWatch stopWatch) : AsyncCommand<InitCommand.InitSettings>
{
    public class InitSettings : CommandSettings
    {
        [CommandOption("-n|--non-interactive")]
        [Description("Runs the command in non-interactive mode.")]
        public bool NonInteractive { get; set; } = false;

        [CommandOption("-p|--password <PASSWORD>")]
        [Description("Sets the password to decrypt the private key.")]
        public string Password { get; set; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, InitSettings settings, CancellationToken cancellationToken)
    {
        console.WriteAppLogo();

        var result = settings.NonInteractive
                ? await ExecuteNonInteractiveAsync(settings)
                : await ExecuteInteractiveAsync();

        console.WriteSuccessLine($"Sigaba file created at: {result.SigabaFileLocation}");
        console.WriteSuccessLine($"Private key created at: {result.PrivateKeyLocation}");

        return 0;
    }

    private Task<InitializationResult> ExecuteNonInteractiveAsync(InitSettings settings)
    {

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new Exception("Error: Password is required in non-interactive mode.");
        }

        return stopWatch.MeasureAsync(() => app.InitAsync(new InitializationOptions(
            SigabaFileOutputDir: fs.NewCwdDirPath(),
            PrivateKeyPassword: settings.Password)));
    }

    private Task<InitializationResult> ExecuteInteractiveAsync()
    {
        var password = console.PromptForPasswordDefinition("Enter a password to protect the private key:");

        return stopWatch.MeasureAsync(() => app.InitAsync(new InitializationOptions(
            SigabaFileOutputDir: fs.NewCwdDirPath(),
            PrivateKeyPassword: password)));
    }

}
