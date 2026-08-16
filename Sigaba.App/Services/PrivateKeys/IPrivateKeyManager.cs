using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal record PrivateKeySaveResult(FilePath OupuptPath);
internal record PrivateKeyLoadResult(PrivateKey PrivateKey, FilePath LoadedFilePath);

internal interface IPrivateKeyManager
{
  Task<PrivateKeySaveResult> SaveAsync(PrivateKey privateKey, string projectId, string password);
  Task<PrivateKeyLoadResult> LoadAsync(DirPath projectRoot, string projectId, string password);
}