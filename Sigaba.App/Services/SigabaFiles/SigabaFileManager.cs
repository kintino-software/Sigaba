using Microsoft.Extensions.Logging;
using Sigaba.App.Exceptions;
using Sigaba.App.Services.SigabaFiles.V1;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.App.Services.SigabaFiles;

internal class SigabaFileManager(ILogger<SigabaFileManager> logger) : ISigabaFileManager
{
    async Task<SigabaFileSaveResult> ISigabaFileManager.SaveAsync(ISigabaFile sigabaFile, DirPath projectRoot)
    {
        var content = sigabaFile switch
        {
            SigabaFileV1 v1 => v1.Serialize(),
            _ => throw new UnknownSigabaFileVersionException(sigabaFile.Version)
        };

        var filePath = projectRoot.CombineAsFile(Constants.SigabaFileName);
        if (filePath.Exists)
            throw new InvalidOperationException($"File '{filePath}' already exists. Overwriting is not allowed.");

        await filePath.WriteAsync(content, overwrite: false); // not allowed to overwrite

        logger.SavedSigabaFile(filePath);

        return new SigabaFileSaveResult(filePath);
    }

    async Task<SigabaFileLoadResult> ISigabaFileManager.LoadAsync(DirPath referenceFolder)
    {
        if (!referenceFolder.TryGetNearestFileWithNameGoingUp(Constants.SigabaFileName, out var sigabaFilePath))
            throw new SigabaFileNotFoundException(referenceFolder.Path);
        logger.FoundSigabaFileAt(sigabaFilePath);

        var content = await sigabaFilePath.ReadAsync();

        var version = JsonHelper.ReadVersionFromJson(content);
        logger.SigabaFileVersion(version);

        ISigabaFile sigabaFile = version switch
        {
            1 => SigabaFileV1.Deserialize(content),
            _ => throw new UnknownSigabaFileVersionException(version)
        };

        logger.LoadedSigabaFile(sigabaFilePath);
        return new SigabaFileLoadResult(sigabaFile, sigabaFilePath);
    }

    ISigabaFile ISigabaFileManager.CreateDefault(PublicKey publicKey)
    {
        var v1 = SigabaFileV1.CreateDefault(publicKey); // TODO: Consider querying through reflection the latest version
        return v1;
    }
}

internal static partial class SigabaFileManagerLoggerExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Saved Sigaba file at ""{Path}"".")]
    public static partial void SavedSigabaFile(this ILogger logger, FilePath path);

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Loaded Sigaba file at ""{Path}"".")]
    public static partial void LoadedSigabaFile(this ILogger logger, FilePath path);

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = @"Found Sigaba file at ""{Path}"".")]
    public static partial void FoundSigabaFileAt(this ILogger logger, FilePath path);

    [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = @"Sigaba file has version ""{Version}"".")]
    public static partial void SigabaFileVersion(this ILogger logger, int version);
}