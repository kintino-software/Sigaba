using Kintino.CipherConf.App.Dependencies;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.IO.Services;

internal class FileOperations(IFileSystem fs) : IFileOperations
{
    ValueTask IFileOperations.CopyWithOverwrite(string originalFilePath, string newFilePath)
    {
        fs.File.Copy(originalFilePath, newFilePath, true);
        return ValueTask.CompletedTask;
    }

    ValueTask<IEnumerable<string>> IFileOperations.GetFilesFromDirectory(string directory, Regex? searchPattern)
    {
        var fileList = new List<string>();
        foreach (var file in fs.Directory.GetFiles(directory))
        {
            var fileName = fs.Path.GetFileName(file);
            if (searchPattern == null || searchPattern.IsMatch(fileName))
            {
                fileList.Add(file);
            }
        }
        return ValueTask.FromResult<IEnumerable<string>>(fileList);
    }

    async ValueTask IFileOperations.WithTempFile(string originalFile, TempFileEditOperation editingOperation, TempFileBeforeDeleteOperation beforeDeleteOperation)
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
