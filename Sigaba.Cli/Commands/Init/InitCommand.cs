using Sigaba.App;
using Sigaba.Primitives.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Init;


internal class InitCommand(ISigabaApp app, IFileSystem fs, IAnsiConsole console) : AsyncCommand<InitSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, InitSettings settings, CancellationToken cancellationToken)
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

    private async Task ExecuteNonInteractiveAsync(InitSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            console.WriteLine("Error: Password is required in non-interactive mode.");
            return;
        }

        await app.InitAsync(new InitializationOptions(
            SigabaFileOutputDir: fs.NewCwdDirPath(),
            PrivateKeyPassword: settings.Password));
    }

    private async Task ExecuteInteractiveAsync()
    {
        var password = console.PromptForPasswordDefinition("Enter a password to protect the private key:");

        await app.InitAsync(new InitializationOptions(
            SigabaFileOutputDir: fs.NewCwdDirPath(),
            PrivateKeyPassword: password));
    }

}
