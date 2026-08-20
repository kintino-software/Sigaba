using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Primitives.FileSystem;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands.Edit;

internal class EditCommand(ISigabaApp app, IFileSystem fs, ITextEditor textEditor, ILogger<EditCommand> logger) : AsyncCommand<EditCommand.EditCommandSettings>
{
    public class EditCommandSettings : CommandSettings
    {
        [CommandArgument(0, "<file>")]
        [Description("The path to the file to edit.")]
        public string File { get; init; } = null!;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, EditCommandSettings settings, CancellationToken cancellationToken)
    {
        var cwd = fs.NewCwdDirPath();
        var filePath = cwd.CombineAsFile(settings.File);

        var result = await app.EditFileAsync(textEditor, filePath);

        logger.LogInformation("File '{filePath}' edited successfully.", result.EditedFilePath);

        return 0;
    }
}
