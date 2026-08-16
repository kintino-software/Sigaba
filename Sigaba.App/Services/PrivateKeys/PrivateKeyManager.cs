using Sigaba.Crypto;
using Sigaba.Primitives;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.App.Services.PrivateKeys;

internal partial class PrivateKeyManager(ICipher cipher, IPrivateKeyPathResolver pathResolver)
{
  private async Task SaveAsync(PrivateKey privateKey, FilePath path, string password)
  {
    if (path.Exists)
    {
      throw new InvalidOperationException($"Private key already exists at {path}.");
    }
    var encryptedPrivateKey = cipher.EncryptWithPassword(new PlainData(privateKey.Bytes), password);
    var content = encryptedPrivateKey.ToBase64();
    await path.WriteAsync(content, overwrite: false, createFolders: true);
  }

  private async Task<PrivateKey?> LoadAsync(FilePath path, string password)
  {
    var privateKeyContent = await path.ReadAsync();
    var encryptedPrivateKey = EncryptedData.FromBase64(privateKeyContent);
    var plainPrivateKey = cipher.DecryptWithPassword(encryptedPrivateKey, password);

    return new PrivateKey(plainPrivateKey);
  }
}

internal partial class PrivateKeyManager : IPrivateKeyManager
{
  async Task<PrivateKeyLoadResult> IPrivateKeyManager.LoadAsync(DirPath projectRoot, string projectId, string password)
  {
    if (pathResolver.GetPossibleLoadingPaths(projectRoot, projectId).FirstOrDefault(p => p.Exists) is FilePath path &&
       await LoadAsync(path, password) is PrivateKey privateKey)
    {
      return new PrivateKeyLoadResult(privateKey, path);
    }
    throw new InvalidOperationException($"No private key file found.");
  }

  async Task<PrivateKeySaveResult> IPrivateKeyManager.SaveAsync(PrivateKey privateKey, string projectId, string password)
  {
    var path = pathResolver.GetDefaultSavePath(projectId);
    await SaveAsync(privateKey, path, password);
    return new PrivateKeySaveResult(path);
  }
}
