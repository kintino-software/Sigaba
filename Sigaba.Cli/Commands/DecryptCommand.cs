using Sigaba.App;
using Sigaba.Primitives;
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
        [CommandOption("-k|--private-key-location <PATH>")]
        [Description("The preferred location of the private key.")]
        public string PrivatePreferedKeyLocation { get; set; } = string.Empty;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        DirPath? privateKeyLocation = string.IsNullOrWhiteSpace(settings.PrivatePreferedKeyLocation)
            ? null
            : fs.NewDirPath(settings.PrivatePreferedKeyLocation);

        await app.DecipherFilesAsync(
            fs.NewCwdDirPath(),
            settings.Password,
            privateKeyLocation);

        return 0;
    }
}
