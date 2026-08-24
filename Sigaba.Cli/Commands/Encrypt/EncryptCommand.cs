using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Primitives.FileSystem;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Encrypt;

internal class EncryptCommand(
    IGlobalOptions globalOptions,
    ISigabaApp app,
    IFileSystem fs,
    CliStopWatch stopWatch,
    ILogger<EncryptCommand> logger) : BaseCommand(globalOptions)
{
    protected override async Task<int> ExecuteAsyncCore(CommandContext context, BaseCommandSettings settings, CancellationToken cancellationToken)
    {
        var result = await stopWatch.MeasureAsync(() => app.CipherFilesAsync(fs.NewCwdDirPath()));

        logger.LogInformation("{count} file(s) encrypted:", result.PathsOfFilesAffected.Count());
        foreach (var file in result.PathsOfFilesAffected)
        {
            logger.LogInformation("  {file}", file);
        }

        return 0;
    }
}
