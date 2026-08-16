using Sigaba.App;
using Sigaba.Cli.Services.Diagnostics;
using Sigaba.Primitives.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Decrypt;

internal class DecryptCommand(ISigabaApp app, IFileSystem fs, IAnsiConsole console, CliStopWatch stopWatch) : AsyncCommand<DecryptCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-p|--password <PASSWORD>")]
        [Description("The password to decrypt the private key.")]
        public required string? Password { get; set; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new Exception("Password is required to decrypt files.");
        }

        var result = await stopWatch.MeasureAsync(() => app.DecipherFilesAsync(
            fs.NewCwdDirPath(),
            settings.Password));

        console.WriteSuccessLine($"{result.PathsOfFilesAffected.Count()} file(s) decrypted:");
        foreach (var file in result.PathsOfFilesAffected)
        {
            console.WriteSuccessLine($"    {file}");
        }

        return 0;
    }
}
