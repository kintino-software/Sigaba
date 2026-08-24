using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Cli.Models;
using Sigaba.Primitives.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Init;

internal class InitCommand(
    IGlobalOptions globalOptions,
    ISigabaApp app,
    IFileSystem fs,
    IAnsiConsole console,
    ILogger<InitCommand> logger) : BaseCommand<InitCommand.InitSettings>(globalOptions)
{
    public class InitSettings : BaseCommandSettings
    {
        [CommandOption("-n|--non-interactive")]
        [Description("Runs the command in non-interactive mode.")]
        public bool NonInteractive { get; set; } = false;

        [CommandOption("-p|--password <PASSWORD>")]
        [Description("Sets the password to decrypt the private key.")]
        public string Password { get; set; } = string.Empty;

        [CommandOption("--no-logo")]
        [Description("Disables the display of the logo.")]
        public bool NoLogo { get; set; } = false;
    }

    protected override ValidationResult Validate(CommandContext context, InitSettings settings)
    {
        if (settings.NonInteractive && string.IsNullOrWhiteSpace(settings.Password))
        {
            return ValidationResult.Error("Password is required in non-interactive mode.");
        }

        return ValidationResult.Success();
    }

    protected override async Task<int> ExecuteCoreAsync(CommandContext context, InitSettings settings, CancellationToken cancellationToken)
    {
        if (!settings.NoLogo)
            console.Write(new FigletText("Sigaba"));

        var result = settings.NonInteractive
            ? await ExecuteNonInteractiveAsync(settings)
            : await ExecuteInteractiveAsync();

        logger.LogInformation("Sigaba file created at: {location}", result.SigabaFileLocation);
        logger.LogInformation("Private key created at: {location}", result.PrivateKeyLocation);

        return 0;
    }

    private Task<InitializationResult> ExecuteNonInteractiveAsync(InitSettings settings)
    {
        return app.InitAsync(new InitializationOptions(SigabaFileOutputDir: fs.NewCwdDirPath(), PrivateKeyPassword: settings.Password));
    }

    private Task<InitializationResult> ExecuteInteractiveAsync()
    {
        var password = console.PromptForPasswordDefinition("Enter a password to protect the private key:");

        return app.InitAsync(new InitializationOptions(SigabaFileOutputDir: fs.NewCwdDirPath(), PrivateKeyPassword: password));
    }

}
