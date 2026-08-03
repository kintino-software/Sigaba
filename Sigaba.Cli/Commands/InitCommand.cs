using Sigaba.App;
using Sigaba.Cli.Adaptors.SpectreConsole;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Commands;

internal class InitCommand(IEncryptConfigApp app) : CommandWithGlobalSettings
{
    protected override Task<int> ExecuteAsync(CommandContext context, GlobalSettings settings, CancellationToken cancellationToken)
    {
        return TryRunAsync(app.InitAsync);
    }
}
