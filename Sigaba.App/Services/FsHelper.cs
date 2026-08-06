using System.IO.Abstractions;
using Sigaba.App.Services;

namespace Sigaba.App.Services;


internal class FsHelper(IFileSystem fs) : IFsHelper
{
    async Task IFsHelper.WithTempFileAsync(string originalFile, Func<string, Task> editingOperation, Func<string, Task> beforeDeleteOperation)
    {
        var tempFolder = CreateAppFolderIfNotExists("temp");
        var extension = fs.Path.GetExtension(originalFile);
        var fileName = $"originalFile_{Guid.NewGuid()}{(string.IsNullOrEmpty(extension) ? "" : $".{extension}")}";
        var tempFilePath = fs.Path.Combine(tempFolder, fileName);
        fs.File.Copy(originalFile, tempFilePath, true);
        try
        {
            await editingOperation.Invoke(tempFilePath);
            await beforeDeleteOperation.Invoke(tempFilePath);
        }
        catch
        {
            throw;
        }
        finally
        {
            SafeDeleteFile(tempFilePath);
        }
    }

    Task IFsHelper.CopyAndOverwrite(string sourceFilePath, string destinationFilePath)
    {
        fs.File.Copy(sourceFilePath, destinationFilePath, true);
        return Task.CompletedTask;
    }

    // helper methods

    private string CreateAppFolderIfNotExists(params string[] subFolders)
    {
        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = fs.Path.Combine([basePath, "dotnet-ec-library", .. subFolders]);
        if (!fs.Directory.Exists(path))
        {
            fs.Directory.CreateDirectory(path);
        }
        return path;
    }

    private void SafeDeleteFile(string filePath)
    {
        if (fs.File.Exists(filePath))
        {
            fs.File.Delete(filePath);
        }
    }
}
