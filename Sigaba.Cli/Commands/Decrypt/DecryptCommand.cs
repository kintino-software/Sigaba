using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
using Sigaba.Primitives.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Decrypt;

internal class DecryptCommand(
    IGlobalOptions globalOptions,
    ISigabaApp app,
    IFileSystem fs,
    ILogger<DecryptCommand> logger) : BaseCommand<DecryptCommand.Settings>(globalOptions)
{
    public class Settings : BaseCommandSettings
    {
        [CommandOption("-p|--password <PASSWORD>")]
        [Description("The password to decrypt the private key.")]
        public required string Password { get; set; }
    }

    protected override ValidationResult Validate(CommandContext context, Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            return ValidationResult.Error("Password is required to decrypt files.");
        }
        return ValidationResult.Success();
    }

    protected override async Task<int> ExecuteCoreAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var result = await app.DecipherFilesAsync(fs.NewCwdDirPath(), settings.Password);
        logger.LogCipherResult(LogLevel.Information, result);
        return 0;
    }
}
