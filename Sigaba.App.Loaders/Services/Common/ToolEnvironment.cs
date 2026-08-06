using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Sigaba.App.Services.Common;

public class ToolEnvironment(IFileSystem fs)
{
    private string? projectRootCache;

    public string? GetProjectRootDir()
    {
        if (projectRootCache != null)
            return projectRootCache;

        var currentDirectory = fs.Directory.GetCurrentDirectory();
        while (!fs.File.Exists(fs.Path.Combine(currentDirectory, Constants.ToolSettingsFileName)))
        {
            currentDirectory = fs.Path.GetDirectoryName(currentDirectory);
            if (string.IsNullOrEmpty(currentDirectory))
                return null;
        }
        projectRootCache = currentDirectory;
        return projectRootCache;
    }

    public string GetRequiredProjectRootDir()
    {
        return GetProjectRootDir()
            ?? throw new InvalidOperationException("Project root directory not found.");
    }

    public string GetCwd() => fs.Directory.GetCurrentDirectory();

    public bool TryGetNearestFile(string fileName, [NotNullWhen(true)] out string? filePath)
    {
        filePath = null;
        var currentDirectory = fs.Directory.GetCurrentDirectory();
        while (!fs.File.Exists(fs.Path.Combine(currentDirectory, fileName)))
        {
            currentDirectory = fs.Path.GetDirectoryName(currentDirectory);
            if (string.IsNullOrEmpty(currentDirectory))
                return false;
        }
        filePath = fs.Path.Combine(currentDirectory, fileName);
        return true;
    }

    public string GetRequiredNearestFile(string fileName)
    {
        return TryGetNearestFile(fileName, out var filePath) && filePath != null
            ? filePath
            : throw new FileNotFoundException($"File '{fileName}' not found in current or parent directories.");
    }

    public string GetOrCreateToolSystemFolder()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var toolSystemFolder = fs.Path.Combine(appData, Constants.ToolSystemFolderName);
        if (!fs.Directory.Exists(toolSystemFolder))
            fs.Directory.CreateDirectory(toolSystemFolder);
        return toolSystemFolder;
    }
}
