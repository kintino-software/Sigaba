using Microsoft.Extensions.Logging;
using Sigaba.Primitives;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal partial class PrivateKeyLocationResolver(
    IFileSystem fs,
    IEnvironmentVariables env,
    ILogger<PrivateKeyLocationResolver> logger)
{
    private FilePath GetPathFromSystemFolderWithId(Guid guid)
    {
        return fs.NewFilePath($"{Constants.SigabaSystemDir}/{guid:N}/{Constants.PrivateKeyFileName}");
    }

    private FilePath? GetPathFromEnvVar()
    {
        var envVar = env.GetEnvironmentVariable(Constants.PrivateKeyDirEnvVarKey);
        if (string.IsNullOrEmpty(envVar))
            return null;

        return fs.NewFilePath($"{envVar}/{Constants.PrivateKeyFileName}");
    }

    private FilePath GetPathFromCurrentDir()
    {
        return fs.NewFilePath($"{fs.Directory.GetCurrentDirectory()}/{Constants.PrivateKeyFileName}");
    }

    private static FilePath GetPathFromCustomLocation(DirPath customLocation)
    {
        return customLocation.CombineAsFile(Constants.PrivateKeyFileName);
    }

    private IEnumerable<FilePath> GetFallbackLocations(Guid projectId)
    {
        // #1: if the env var is set, the user explicitly wants to use that path, so we should check it first
        var envVarPath = GetPathFromEnvVar(); 
        if (envVarPath != null)
            yield return envVarPath;
        
        // #2: the file is in the cwd, so looks like the user is running the app from the project folder, so we should check it next
        yield return GetPathFromCurrentDir();
        
        // #3: the file is in the system folder with the project id. as it is a very specific location, we should check it next
        yield return GetPathFromSystemFolderWithId(projectId);
        
    }
}

internal partial class PrivateKeyLocationResolver : IPrivateKeyLocationResolver
{
    FilePath IPrivateKeyLocationResolver.GetLoadPath(Guid projectId, DirPath? customLocation)
    {
        if(customLocation != null)
        {
            var path = GetPathFromCustomLocation(customLocation);
            if(!path.Exists)
                throw new FileNotFoundException($"The private key file was not found in the custom location: {path}");
            return path;
        }

        foreach (var path in GetFallbackLocations(projectId))
        {
            logger.LogInformation("Checking for private key in fallback location: {path}", path);
            if (path.Exists)
                return path;
        }
        throw new FileNotFoundException("Private key not found in any of the fallback locations.");
    }

    FilePath IPrivateKeyLocationResolver.GetSavePath(Guid projectId, DirPath? customLocation)
    {
        return customLocation == null ? GetPathFromSystemFolderWithId(projectId) : GetPathFromSystemFolderWithId(projectId);
    }
}
