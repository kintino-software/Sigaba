using Sigaba.Primitives;

namespace Sigaba.App.Services.SigabaFiles;

internal interface ISigabaFileManager
{
    Task<ISigabaFile?> LoadAsync(FilePath path);
    Task SaveAsync(ISigabaFile sigabaFile, FilePath path);
    ISigabaFile CreateDefault(PublicKey publicKey);
}
