using Microsoft.Extensions.Logging;
using Sigaba.App.Exceptions;
using Sigaba.App.Services.SigabaFiles.V1;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Sigaba.App.Services.SigabaFiles;

internal record SigabaFileSaveResult(FilePath OutputPath);
internal record SigabaFileLoadResult(ISigabaFile SigabaFile, FilePath SigabaFilePath);

internal interface ISigabaFileManager
{
    Task<SigabaFileSaveResult> SaveAsync(ISigabaFile sigabaFile, DirPath projectRoot);
    Task<SigabaFileLoadResult> LoadAsync(DirPath referenceFolder);
    ISigabaFile CreateDefault(PublicKey publicKey);
}


internal class SigabaFileManager(IFileSystem fs, ILogger<SigabaFileManager> logger) : ISigabaFileManager
{
    async Task<SigabaFileSaveResult> ISigabaFileManager.SaveAsync(ISigabaFile sigabaFile, DirPath projectRoot)
    {
        var content = sigabaFile switch
        {
            SigabaFileV1 v1 => v1.Serialize(),
            _ => throw new UnknownSigabaFileVersionException(sigabaFile.Version)
        };

        var filePath = new FilePath(projectRoot, Constants.SigabaFileName);
        await fs.SafeWriteAllTextAsync(filePath, content, allowOverwrite: false);
        logger.SavedSigabaFile(filePath);

        return new SigabaFileSaveResult(filePath);
    }

    async Task<SigabaFileLoadResult> ISigabaFileManager.LoadAsync(DirPath referenceFolder)
    {
        if (!TryGetNearestFileWithNameGoingUp(referenceFolder, Constants.SigabaFileName, out var sigabaFilePath))
            throw new SigabaFileNotFoundException(referenceFolder);
        logger.FoundSigabaFileAt(sigabaFilePath);

        var content = await fs.File.ReadAllTextAsync(sigabaFilePath);

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

    // helpers


    public bool TryGetNearestFileWithNameGoingUp(DirPath referenceFolder, string fileName, [NotNullWhen(true)] out FilePath? foundFilePath)
    {
        foundFilePath = null;

        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        if (!fs.Directory.Exists(referenceFolder))
            return false;

        for (var curDir = referenceFolder; curDir != null; curDir = curDir.Parent())
        {
            var filePath = new FilePath(curDir, fileName);
            if (fs.File.Exists(filePath))
            {
                foundFilePath = filePath;
                return true;
            }
        }

        return false;
    }
}

[ExcludeFromCodeCoverage]
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