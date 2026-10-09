using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
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
        var result = await app.CipherFilesAsync(fs.Directory.GetCurrentDirectory());
        logger.LogCipherResult(LogLevel.Information, result);
        return 0;
    }
}
