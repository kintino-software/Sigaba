using Sigaba.App;
using Sigaba.Cli.Adaptors.SpectreConsole;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Sigaba.Cli.Commands;

internal class EditCommand(IEncryptConfigApp app, IFileSystem fs, ITextEditor textEditor) : CommandWithGlobalSettings<EditCommand.EditCommandSettings>
{
    public class EditCommandSettings : GlobalSettings
    {
        [CommandOption("-f|--file")]
        [Description("The path to the file to edit.")]
        public string File { get; set; } = null!;
    }

    protected override Task<int> ExecuteAsync(CommandContext context, EditCommandSettings settings, CancellationToken cancellationToken)
    {
        var projectFolder = GetProjectTargetDir(settings, fs);
        var cwd = fs.Directory.GetCurrentDirectory();
        var filePath = fs.Path.Combine(cwd, settings.File);
        return TryRunAsync(() => app.EditFileAsync(textEditor, filePath));
    }
}
