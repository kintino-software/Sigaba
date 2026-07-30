using Kintino.CipherConf.App;
using Kintino.CipherConf.Cli.Adaptors.SpectreConsole;
using Spectre.Console.Cli;

namespace Kintino.CipherConf.Cli.Commands;

internal class EncryptCommand(IEncryptConfigApp app) : CommandWithGlobalSettings
{
    protected override Task<int> ExecuteAsync(CommandContext context, GlobalSettings settings, CancellationToken cancellationToken)
    {
        return TryRunAsync(app.CipherFilesAsync);
    }
}
