using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
using Sigaba.Primitives.FileSystem;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Encrypt;

internal class EncryptCommand(
    IGlobalOptions globalOptions,
    ISigabaApp app,
    IFileSystem fs,
    ILogger<EncryptCommand> logger) : BaseCommand(globalOptions)
{
    protected override async Task<int> ExecuteCoreAsync(CommandContext context, BaseCommandSettings settings, CancellationToken cancellationToken)
    {
        var result = await app.CipherFilesAsync(fs.NewCwdDirPath());
        logger.LogCipherResult(LogLevel.Information, result);
        return 0;
    }
}
