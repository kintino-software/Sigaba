using Sigaba.App;
using Sigaba.Cli.Adaptors.SpectreConsole;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Commands;

internal class EncryptCommand(IEncryptConfigApp app) : CommandWithGlobalSettings
{
    protected override Task<int> ExecuteAsync(CommandContext context, GlobalSettings settings, CancellationToken cancellationToken)
    {
        return TryRunAsync(app.CipherFilesAsync);
    }
}
