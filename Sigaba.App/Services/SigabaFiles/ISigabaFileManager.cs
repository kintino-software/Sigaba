using Sigaba.Primitives;

namespace Sigaba.App.Services.SigabaFiles;

internal interface ISigabaFileManager
{
    Task<ISigabaFile> LoadAsync(FilePath filePath);
    Task SaveAsync(ISigabaFile sigabaFile, FilePath filePath);
    ISigabaFile CreateDefault(PublicKey publicKey);
}
