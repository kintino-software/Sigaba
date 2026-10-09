using Microsoft.Extensions.Logging;
using Sigaba.Primitives.FileSystem;
using Sigaba.Services;
using System.Diagnostics.CodeAnalysis;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyPathResolver
{
    FilePath GetDefaultSavePath(string projectId);
    IEnumerable<FilePath> GetPossibleLoadingPaths(DirPath projectRootPath, string projectId);
}

internal class PrivateKeyPathResolver(
    IEnvironmentVariables env,
    ILogger<PrivateKeyPathResolver> logger)
    : IPrivateKeyPathResolver
{
    public const string PrivateKeyDirEnvVarKey = "SIGABA_PRIVATE_KEY_DIR";
    public const string PrivateKeyFileName = "private.key";
    public static readonly string SigabaSystemDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ".sigaba");

    FilePath IPrivateKeyPathResolver.GetDefaultSavePath(string projectId)
    {
        return GetDefaultPrivateKeyOutputPath(projectId);
    }

    IEnumerable<FilePath> IPrivateKeyPathResolver.GetPossibleLoadingPaths(DirPath projectRootPath, string projectId)
    {
        // by order of precedence:

        // #1. Get from environment variable
        var envVar = env.GetEnvironmentVariable(PrivateKeyDirEnvVarKey);
        if (envVar != null)
        {
            var dir = new DirPath(envVar);
            var filePath = new FilePath(dir, PrivateKeyFileName);
            logger.TryingGetPrivateKeyPathFrom(filePath);
            yield return filePath;
        }
        else
        {
            logger.EnvironmentVariableNotFound(PrivateKeyDirEnvVarKey);
        }

        // #2. Get from project directory
        var projectRootFilePath = new FilePath(projectRootPath, PrivateKeyFileName);
        logger.TryingGetPrivateKeyPathFrom(projectRootFilePath);
        yield return projectRootFilePath;


        // #3. Get from default system directory
        var defaultSystemFilePath = GetDefaultPrivateKeyOutputPath(projectId);
        logger.TryingGetPrivateKeyPathFrom(defaultSystemFilePath);
        yield return defaultSystemFilePath;
    }

    // helpers

    private static FilePath GetDefaultPrivateKeyOutputPath(string projectId)
    {
        return new FilePath(SigabaSystemDir, projectId, PrivateKeyFileName);
    }
}

[ExcludeFromCodeCoverage]
public static partial class PrivateKeyPathResolverLogExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Trying to get private key path from ""{location}"".")]
    public static partial void TryingGetPrivateKeyPathFrom(this ILogger logger, FilePath location);

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Environment variable ""{envVarKey}"" not found.")]
    public static partial void EnvironmentVariableNotFound(this ILogger logger, string envVarKey);
}