using Sigaba.App;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class DecryptCommand(ISigabaApp app, IFileSystem fs) : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await app.DecipherFilesAsync(fs.Directory.GetCurrentDirectory());
        return 0;
    }
}
