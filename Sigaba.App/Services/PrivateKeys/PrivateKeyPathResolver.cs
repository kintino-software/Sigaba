using Microsoft.Extensions.Logging;
using Sigaba.Primitives.FileSystem;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal class PrivateKeyPathResolver(
    IFileSystem fs,
    IEnvironmentVariables env,
    ILogger<PrivateKeyPathResolver> logger)
    : IPrivateKeyPathResolver
{

    private FilePath GetDefaultPrivateKeyOutputPath(string projectId)
    {
        return fs.NewFilePath(Constants.SigabaSystemDir, projectId, Constants.PrivateKeyFileName);
    }

    // interface impl

    FilePath IPrivateKeyPathResolver.GetDefaultSavePath(string projectId)
    {
        return GetDefaultPrivateKeyOutputPath(projectId);
    }

    IEnumerable<FilePath> IPrivateKeyPathResolver.GetPossibleLoadingPaths(DirPath projectRootPath, string projectId)
    {
        // by order of precedence:

        // #1. Get from environment variable
        var envVar = env.GetEnvironmentVariable(Constants.PrivateKeyDirEnvVarKey);
        if (envVar != null)
        {
            var filePath = fs.NewFilePath(envVar, Constants.PrivateKeyFileName);
            logger.TryingGetPrivateKeyPathFrom(filePath.AbsolutePath);
            yield return filePath;
        }
        else
        {
            logger.EnvironmentVariableNotFound(Constants.PrivateKeyDirEnvVarKey);
        }

        // #2. Get from project directory
        var projectRootFilePath = projectRootPath.CombineAsFile(Constants.PrivateKeyFileName);
        logger.TryingGetPrivateKeyPathFrom(projectRootFilePath.AbsolutePath);
        yield return projectRootFilePath;


        // #3. Get from default system directory
        var defaultSystemFilePath = GetDefaultPrivateKeyOutputPath(projectId);
        logger.TryingGetPrivateKeyPathFrom(defaultSystemFilePath.AbsolutePath);
        yield return defaultSystemFilePath;
    }
}

public static partial class PrivateKeyPathResolverLogExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Trying to get private key path from ""{location}"".")]
    public static partial void TryingGetPrivateKeyPathFrom(this ILogger logger, string location);

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Environment variable ""{envVarKey}"" not found.")]
    public static partial void EnvironmentVariableNotFound(this ILogger logger, string envVarKey);
}