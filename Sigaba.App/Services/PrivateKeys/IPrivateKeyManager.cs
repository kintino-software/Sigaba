using Microsoft.Extensions.Logging;
using Sigaba.Crypto;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Sigaba.App.Services.PrivateKeys;

internal record PrivateKeySaveResult(FilePath OutputPath);
internal record PrivateKeyLoadResult(PrivateKey PrivateKey, FilePath LoadedFilePath);

internal interface IPrivateKeyManager
{
    Task<PrivateKeySaveResult> SaveAsync(PrivateKey privateKey, string projectId, string password);
    Task<PrivateKeyLoadResult> LoadAsync(DirPath projectRoot, string projectId, string password);
    Task<PrivateKeyLoadResult> LoadAsync(FilePath filePath, string password);
}


internal class PrivateKeyManager(
    IFileSystem fs,
    ICipher cipher,
    IPrivateKeyPathResolver pathResolver,
    ILogger<PrivateKeyManager> logger)
    : IPrivateKeyManager
{
    async Task<PrivateKeyLoadResult> IPrivateKeyManager.LoadAsync(DirPath projectRoot, string projectId, string password)
    {
        var resolvedPath = pathResolver
            .GetPossibleLoadingPaths(projectRoot, projectId)
            .FirstOrDefault(p => fs.File.Exists(p))
            ?? throw new InvalidOperationException($"Private key not found on any of expected locations.");

        var privateKey = await LoadAsync(resolvedPath, password);

        return new PrivateKeyLoadResult(privateKey, resolvedPath);
    }

    async Task<PrivateKeySaveResult> IPrivateKeyManager.SaveAsync(PrivateKey privateKey, string projectId, string password)
    {
        var path = pathResolver.GetDefaultSavePath(projectId);
        await SaveAsync(privateKey, path, password);
        return new PrivateKeySaveResult(path);
    }

    async Task<PrivateKeyLoadResult> IPrivateKeyManager.LoadAsync(FilePath filePath, string password)
    {
        var privateKey = await LoadAsync(filePath, password);
        return new PrivateKeyLoadResult(privateKey, filePath);
    }

    // helpers

    private async Task SaveAsync(PrivateKey privateKey, FilePath path, string password)
    {
        var encryptedPrivateKey = cipher.EncryptWithPassword(new PlainData(privateKey.Bytes), password);
        var content = encryptedPrivateKey.ToBase64();
        await fs.SafeWriteAllTextAsync(path, content, allowOverwrite: false);
        logger.SavedPrivateKey(path);
    }

    private async Task<PrivateKey> LoadAsync(FilePath path, string password)
    {
        var privateKeyContent = await fs.File.ReadAllTextAsync(path);

        logger.ReadPrivateKey(path);

        var encryptedPrivateKey = EncryptedData.FromBase64(privateKeyContent);
        var plainPrivateKey = cipher.DecryptWithPassword(encryptedPrivateKey, password);

        return new PrivateKey(plainPrivateKey);
    }
}

[ExcludeFromCodeCoverage]
internal static partial class PrivateKeyManagerLogExtensions
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = @"Saved private key at ""{Path}"".")]
    public static partial void SavedPrivateKey(this ILogger logger, FilePath path);

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = @"Loaded private key from ""{Path}"".")]
    public static partial void LoadedPrivateKey(this ILogger logger, FilePath path);

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = @"Read private key from ""{Path}"".")]
    public static partial void ReadPrivateKey(this ILogger logger, FilePath path);
}