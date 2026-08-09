using Sigaba.App;
using Sigaba.Cli.Interactive;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class InitCommand(ISigabaApp app, IFileSystem fs, InteractiveInit interactiveInit) : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        interactiveInit.Run().Deconstruct(out var privateKeyPassword, out var privateKeyDir);

        await app.InitAsync(new()
        {
            PrivateKeyPassword = privateKeyPassword,
            SigabaFileOutputDir = fs.Directory.GetCurrentDirectory(),

        });
        return 0;
    }
}
