using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Sigaba.Crypto;
using Sigaba.Primitives;
using Sigaba.Services;

namespace Sigaba.App.Services.PrivateKeys;

internal partial class PrivateKeyManager(
    ICipher cipher,
    IPrivateKeyLocationResolver privateKeyLocationResolver,
    ILogger<PrivateKeyManager> logger)
{
    
   

}

internal partial class PrivateKeyManager : IPrivateKeyManager
{
    async Task IPrivateKeyManager.SaveAsync(Guid projectId, PrivateKey privateKey, string password, DirPath? customLocation)
    {
        var destination = privateKeyLocationResolver.GetSavePath(projectId, customLocation);
        if(destination.Exists)
            throw new InvalidOperationException($"Private key already exists at: {destination}");

        var encryptedPrivateKey = cipher.EncryptWithPassword(privateKey, password);
        var content = encryptedPrivateKey.ToBase64();
        await destination.WriteAsync(content, overwrite: false, createFolders: true);

        logger.LogInformation("Private key saved to: {destination}", destination);
    }

    async Task<PrivateKey> IPrivateKeyManager.LoadAsync(Guid projectId, string password, DirPath? customLocation)
    {
        var filePath = privateKeyLocationResolver.GetLoadPath(projectId, customLocation);

        var privateKeyContent = await filePath.ReadAsync();
        var encryptedPrivateKey = EncryptedData.FromBase64(privateKeyContent);
        var plainPrivateKey = cipher.DecryptWithPassword(encryptedPrivateKey, password);

        logger.LogInformation("Private key loaded from: {filePath}", filePath);
        return new PrivateKey(plainPrivateKey);
    }
}
