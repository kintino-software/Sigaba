using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Primitives.FileSystem;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Decrypt;

internal class DecryptCommand(
    IGlobalOptions globalOptions,
    ISigabaApp app,
    IFileSystem fs,
    CliStopWatch stopWatch,
    ILogger<DecryptCommand> logger) : BaseCommand<DecryptCommand.Settings>(globalOptions)
{
    public class Settings : BaseCommandSettings
    {
        [CommandOption("-p|--password <PASSWORD>")]
        [Description("The password to decrypt the private key.")]
        public required string? Password { get; set; }
    }

    protected override async Task<int> ExecuteAsyncCore(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new Exception("Password is required to decrypt files.");
        }

        var result = await stopWatch.MeasureAsync(() => app.DecipherFilesAsync(
            fs.NewCwdDirPath(),
            settings.Password));

        logger.LogInformation("{count} file(s) decrypted:", result.PathsOfFilesAffected.Count());
        foreach (var file in result.PathsOfFilesAffected)
        {
            logger.LogInformation("  - {file}", file);
        }

        return 0;
    }
}
