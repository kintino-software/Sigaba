using Sigaba.Primitives;

namespace Sigaba.App.Services.SigabaFiles;

internal interface ISigabaFileManager
{
    Task<ISigabaFile> LoadAsync(string filePath);
    Task SaveAsync(ISigabaFile sigabaFile, string filePath);
    ISigabaFile CreateDefault(PublicKey publicKey);
}
