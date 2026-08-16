using Sigaba.App;
using Sigaba.Primitives;
using System.Diagnostics;

namespace Sigaba.Cli.Services;

internal class WindowsEditTextEditor : ITextEditor
{
  public async Task EditFile(FilePath filePath)
  {
    try
    {
      var process = Process.Start("edit", $"\"{filePath.Path}\"");
      await process.WaitForExitAsync();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Error editing file.", ex);
    }
  }
}
