using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal class PrivateKeyManager(ICipher cipher) : IPrivateKeyManager
{
    async Task IPrivateKeyManager.SaveAsync(PrivateKey privateKey, FilePath path, string password)
    {
        if (path.Exists)
            throw new InvalidOperationException($"Private key already exists at: {path}");

        var encryptedPrivateKey = cipher.EncryptWithPassword(privateKey, password);
        var content = encryptedPrivateKey.ToBase64();
        await path.WriteAsync(content, overwrite: false, createFolders: true);
    }

    async Task<PrivateKey?> IPrivateKeyManager.LoadAsync(FilePath path, string password)
    {
        if (!path.Exists)
            return null;

        var privateKeyContent = await path.ReadAsync();
        var encryptedPrivateKey = EncryptedData.FromBase64(privateKeyContent);
        var plainPrivateKey = cipher.DecryptWithPassword(encryptedPrivateKey, password);

        return new PrivateKey(plainPrivateKey);
    }
}
