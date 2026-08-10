using Sigaba.App;
using Sigaba.Cli.Interactive;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class InitCommand(ISigabaApp app, IFileSystem fs, InteractiveInit interactiveInit) : AsyncCommand<InitCommand.Settings>
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
        string? privateKeyPassword = null;
        string? privateKeyDir = null;

        interactiveInit.Run().Deconstruct(out privateKeyPassword, out privateKeyDir);

        await app.InitAsync(new()
        {
            PrivateKeyPassword = privateKeyPassword,
            SigabaFileOutputDir = fs.Directory.GetCurrentDirectory(),

        });
        return 0;
    }
}
