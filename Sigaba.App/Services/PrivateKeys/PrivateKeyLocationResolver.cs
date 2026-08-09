using Microsoft.Extensions.Logging;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal sealed partial class PrivateKeyLocationResolver(
    IFileSystem fs,
    IEnvironmentVariables env,
    ILogger<PrivateKeyLocationResolver> logger)
{
    private string CombinePathWithSystemFolderWithId(Guid projectId)
    {
        var systemFolderWithId = fs.Path.Combine(Constants.SigabaSystemDir, projectId.ToString("N"));
        return systemFolderWithId;
    }

    private string? GetFilePathFromEnvVars()
    {
        logger.LogDebug(
            "Checking for environemnt variable {key} for private key location",
            Constants.PrivateKeyDirEnvVarKey);
        var dirPath = env.GetEnvironmentVariable(Constants.PrivateKeyDirEnvVarKey);
        if (string.IsNullOrEmpty(dirPath))
        {
            return null;
        }

        var filePath = fs.Path.Combine(dirPath, Constants.PrivateKeyFileName);
        if (!string.IsNullOrEmpty(dirPath) || fs.File.Exists(fs.Path.Combine(filePath)))
        {
            logger.LogInformation("Found private key through environment variable {key}", Constants.PrivateKeyDirEnvVarKey);
            return filePath;
        }

        return null;
    }

    private string? GetFilePathFromCwd()
    {
        var cwd = fs.Directory.GetCurrentDirectory();
        logger.LogDebug("Checking {cwd} for private key location", cwd);
        var filePath = fs.Path.Combine(cwd, Constants.PrivateKeyFileName);
        if (fs.File.Exists(filePath))
        {
            logger.LogInformation("Found private key in current working directory");
            return filePath;
        }
        return null;
    }

    private string? GetFilePathFromSigabaFolder()
    {
        var sigabaFolderPath = Constants.SigabaSystemDir;
        logger.LogDebug("Checking {sigabaFolderPath} for private key location", sigabaFolderPath);
        var filePath = fs.Path.Combine(sigabaFolderPath, Constants.PrivateKeyFileName);
        if (fs.File.Exists(filePath))
        {
            logger.LogInformation("Found private key in Sigaba system folder");
            return filePath;
        }
        logger.LogDebug("Could not find private key in Sigaba system folder");
        return null;
    }

    private string? GetFilePathFromSigabaFolderWithId(Guid projectId)
    {
        var filePath = CombinePathWithSystemFolderWithId(projectId);
        logger.LogDebug("Checking {filePath} for private key...", filePath);

        if (fs.File.Exists(filePath))
        {
            logger.LogInformation("Found private key in Sigaba system folder with project ID");
            return filePath;
        }

        logger.LogDebug("Could not find private key in Sigaba system folder with project ID");
        return null;
    }
}

internal sealed partial class PrivateKeyLocationResolver : IPrivateKeyLocationResolver
{
    string IPrivateKeyLocationResolver.GetDefaultFilePath(Guid projectId)
    {
        return CombinePathWithSystemFolderWithId(projectId);
    }

    string IPrivateKeyLocationResolver.ResolveCurrentLocation(Guid projectId)
    {
        // in order of precedence
        return
            GetFilePathFromCwd() ??
            GetFilePathFromEnvVars() ??
            GetFilePathFromSigabaFolderWithId(projectId) ??
            GetFilePathFromSigabaFolder() ??
            throw new Exception("Private key file not found in any of the expected locations.");
    }
}
