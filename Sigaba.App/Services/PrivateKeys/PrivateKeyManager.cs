using Microsoft.Extensions.Logging;
using Sigaba.Crypto;
using Sigaba.Primitives.Crypto;
using Sigaba.Primitives.FileSystem;
using System.Diagnostics.CodeAnalysis;

namespace Sigaba.App.Services.PrivateKeys;

internal class PrivateKeyManager(
    ICipher cipher,
    IPrivateKeyPathResolver pathResolver,
    ILogger<PrivateKeyManager> logger)
    : IPrivateKeyManager
{
    private async Task SaveAsync(PrivateKey privateKey, FilePath path, string password)
    {
        if (path.Exists)
        {
            throw new InvalidOperationException($"Private key already exists at {path}. Overwriting is not allowed.");
        }
        var encryptedPrivateKey = cipher.EncryptWithPassword(new PlainData(privateKey.Bytes), password);
        var content = encryptedPrivateKey.ToBase64();
        await path.WriteAsync(content, overwrite: false, createFolders: true);
        logger.SavedPrivateKey(path);
    }

    private async Task<PrivateKey> LoadAsync(FilePath path, string password)
    {
        var privateKeyContent = await path.ReadAsync();

        logger.ReadPrivateKey(path);

        var encryptedPrivateKey = EncryptedData.FromBase64(privateKeyContent);
        var plainPrivateKey = cipher.DecryptWithPassword(encryptedPrivateKey, password);

        return new PrivateKey(plainPrivateKey);
    }

    // interface impl

    async Task<PrivateKeyLoadResult> IPrivateKeyManager.LoadAsync(DirPath projectRoot, string projectId, string password)
    {
        var resolvedPath = pathResolver.GetPossibleLoadingPaths(projectRoot, projectId).FirstOrDefault(p => p.Exists)
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