using Sigaba.App;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Commands;

internal class DecryptCommand(IEncryptConfigApp app) : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await app.DecipherFilesAsync();
        return 0;
    }
}
