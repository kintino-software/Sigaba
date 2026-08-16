using Sigaba.App;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Primitives.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Encrypt;

internal class EncryptCommand(ISigabaApp app, IFileSystem fs, IAnsiConsole console, CliStopWatch stopWatch) : AsyncCommand
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var result = await stopWatch.MeasureAsync(() => app.CipherFilesAsync(fs.NewCwdDirPath()));

        console.WriteSuccessLine($"{result.PathsOfFilesAffected.Count()} file(s) encrypted:");
        foreach (var file in result.PathsOfFilesAffected)
        {
            console.WriteSuccessLine($"    {file}");
        }

        return 0;
    }
}
