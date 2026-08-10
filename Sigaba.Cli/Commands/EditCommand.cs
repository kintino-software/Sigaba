using Sigaba.App;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class EditCommand(ISigabaApp app, IFileSystem fs, ITextEditor textEditor) : AsyncCommand<EditCommand.EditCommandSettings>
{
    public class EditCommandSettings : CommandSettings
    {
        [CommandOption("-f|--file")]
        [Description("The path to the file to edit.")]
        public string File { get; init; } = null!;
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, EditCommandSettings settings, CancellationToken cancellationToken)
    {
        var cwd = fs.CreateCwdDir();
        var filePath = cwd.CombineAsFile(settings.File);
        await app.EditFileAsync(textEditor, filePath);
        return 0;
    }
}
