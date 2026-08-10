using Microsoft.Extensions.Logging;
using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal sealed class PrivateKeyManager(
    ICipher cipher,
    IPrivateKeyLocationResolver privateKeyLocationResolver,
    ILogger<PrivateKeyManager> logger) : IPrivateKeyManager
{
    async Task IPrivateKeyManager.SaveAsync(Guid projectId, PrivateKey privateKey, string password)
    {
        var filePath = privateKeyLocationResolver.GetDefaultFilePath(projectId);

        var encryptedPrivateKey = cipher.EncryptWithPassword(privateKey, password);
        var fileContent = encryptedPrivateKey.ToBase64();
        await filePath.WriteAsync(fileContent, overwrite: true);

        logger.LogInformation("Private key saved to: {filePath}", filePath);
    }

    async Task<PrivateKey> IPrivateKeyManager.LoadAsync(Guid projectId, string password)
    {
        var filePath = privateKeyLocationResolver.ResolveCurrentLocation(projectId);

        var privateKeyContent = await filePath.ReadAsync();
        var encryptedPrivateKey = EncryptedData.FromBase64(privateKeyContent);
        var plainPrivateKey = cipher.DecryptWithPassword(encryptedPrivateKey, password);

        logger.LogInformation("Private key loaded from: {filePath}", filePath);
        return new PrivateKey(plainPrivateKey);
    }
}
