using Sigaba.Primitives.FileSystem;
using System.Diagnostics;

namespace Sigaba.Cli.Services.TextEditors;

internal static class TextEditorLauncher
{
    public static async Task Launch(string programName, FilePath filePath)
    {
        try
        {
            var process = Process.Start(programName, $"\"{filePath.Path}\"");
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error launching process '{programName}' with argument '{filePath}'.", ex);
        }
    }
}
