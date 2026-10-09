using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
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

        [CommandOption("-k|--private-key-file <PATH>")]
        [Description("(Optional) The path to the private key file. If none provided, the path will be inferred (see docs).")]
        public string? PrivateKeyFilePath { get; set; } = null;
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
        var result = string.IsNullOrWhiteSpace(settings.PrivateKeyFilePath)
            ? await app.DecipherFilesAsync(fs.Directory.GetCurrentDirectory(), settings.Password)
            : await app.DecipherFilesAsync(settings.PrivateKeyFilePath, settings.Password);
        logger.LogCipherResult(LogLevel.Information, result);
        return 0;
    }
}
