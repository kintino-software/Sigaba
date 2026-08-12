using Sigaba.App;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class InitCommand(ISigabaApp app, IFileSystem fs, IAnsiConsole console) : AsyncCommand<InitCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-n|--non-interactive")]
        [Description("Runs the command in non-interactive mode.")]
        public bool NonInteractive { get; set; } = false;

        [CommandOption("-p|--password <PASSWORD>")]
        [Description("Sets the password to decrypt the private key.")]
        public string Password { get; set; } = string.Empty;

        [CommandOption("-o|--output-private-key <PASSWORD>")]
        [Description("Sets the output directory of the private key.")]
        public string PrivateKeyOutputDir { get; set; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        if (settings.NonInteractive)
        {
            await ExecuteNonInteractiveAsync(settings);
        }
        else
        {
            await ExecuteInteractiveAsync();
        }
        return 0;
    }

    private async Task ExecuteNonInteractiveAsync(Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            console.WriteLine("Error: Password is required in non-interactive mode.");
            return;
        }

        await app.InitAsync(new()
        {
            PrivateKeyPassword = settings.Password,
            SigabaFileOutputDir = fs.NewCwdDirPath(),
            PrivateKeySaveLocation = string.IsNullOrWhiteSpace(settings.PrivateKeyOutputDir) ? null : fs.NewDirPath(settings.PrivateKeyOutputDir)
        });
    }

    private async Task ExecuteInteractiveAsync()
    {
        var password = console.PromptForPasswordDefinition("Enter a password to protect the private key:");
        var privateKeyDir = console.PromptForInput("Enter the output directory for the private key (leave empty to save to default location):");

        await app.InitAsync(new()
        {
            PrivateKeyPassword = password,
            SigabaFileOutputDir = fs.NewCwdDirPath(),
            PrivateKeySaveLocation = string.IsNullOrWhiteSpace(privateKeyDir) ? null : fs.NewDirPath(privateKeyDir)
        });
    }

}
