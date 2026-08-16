using Sigaba.App;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.Cli.Services.TextEditors;

internal class TextEditor : ITextEditor
{
    public Task EditFile(FilePath filePath)
    {
        if (OperatingSystem.IsWindows())
        {
            return TextEditorLauncher.Launch("edit", filePath);
        }
        else if (OperatingSystem.IsLinux())
        {
            return TextEditorLauncher.Launch("vi", filePath);
        }
        else if (OperatingSystem.IsMacOS())
        {
            return TextEditorLauncher.Launch("nano", filePath);
        }
        else
        {
            throw new NotSupportedException("Text editor is not available for this operating system.");
        }
    }
}
