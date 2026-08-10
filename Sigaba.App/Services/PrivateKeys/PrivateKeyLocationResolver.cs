using Microsoft.Extensions.Logging;
using Sigaba.Primitives;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal sealed partial class PrivateKeyLocationResolver(
    IFileSystem fs,
    IEnvironmentVariables env,
    ILogger<PrivateKeyLocationResolver> logger) : IPrivateKeyLocationResolver
{
    FilePath IPrivateKeyLocationResolver.GetDefaultFilePath(Guid projectId)
    {
        return fs.NewDirPath($"{Constants.SigabaSystemDir}/{projectId:N}").CombineAsFile(Constants.PrivateKeyFileName);
    }

    FilePath IPrivateKeyLocationResolver.ResolveCurrentLocation(Guid projectId)
    {
        string?[] dirStrings = [
            fs.Directory.GetCurrentDirectory(),
            env.GetEnvironmentVariable(Constants.PrivateKeyDirEnvVarKey),
            $"{Constants.SigabaSystemDir}/{projectId:N}",
            Constants.SigabaSystemDir
        ];

        foreach (var dir in dirStrings.Where(x => !string.IsNullOrEmpty(x)).Cast<string>())
        {
            var filePath = fs.NewDirPath(dir).CombineAsFile(Constants.PrivateKeyFileName);
            logger.LogDebug("Checking {filePath} for private key...", filePath);
            if (filePath.Exists)
            {
                logger.LogInformation("Found private key at {filePath}", filePath);
                return filePath;
            }
        }

        throw new Exception("Private key file not found in any of the expected locations.");
    }
}
