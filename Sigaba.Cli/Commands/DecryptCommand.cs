using Sigaba.App;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class DecryptCommand(ISigabaApp app, IFileSystem fs) : AsyncCommand<DecryptCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-p|--password <PASSWORD>")]
        [Description("The password to decrypt the private key.")]
        public string Password { get; set; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        await app.DecipherFilesAsync(
            fs.NewCwdDirPath(),
            settings.Password);

        return 0;
    }
}
