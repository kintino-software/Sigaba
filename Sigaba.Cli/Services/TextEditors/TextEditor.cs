using Microsoft.Extensions.Logging;
using Sigaba.App;
using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;

namespace Sigaba.Cli.Services.TextEditors;

internal class TextEditor(ILogger<TextEditor> logger) : ITextEditor
{
    public Task EditFile(FilePath filePath)
    {
        string? programName = OperatingSystem.IsWindows() ? "edit" :
                             OperatingSystem.IsLinux() ? "vi" :
                             OperatingSystem.IsMacOS() ? "nano" :
                             throw new NotSupportedException("Text editor is not available for this operating system.");

        logger.TryingLaunchEditor(programName, filePath);

        return TextEditorLauncher.Launch(programName, filePath);
    }
}

[ExcludeFromCodeCoverage]
public static partial class TextEditorLauncherLoggerExtensions
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Debug,
        Message = "Launching text editor '{ProgramName}' for file '{FilePath}'")]
    public static partial void TryingLaunchEditor(this ILogger logger, string programName, FilePath filePath);
}