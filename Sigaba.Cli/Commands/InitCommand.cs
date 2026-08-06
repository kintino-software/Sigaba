using Sigaba.App;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Commands;

internal class InitCommand(ISigabaApp app) : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await app.InitAsync();
        return 0;
    }
}
