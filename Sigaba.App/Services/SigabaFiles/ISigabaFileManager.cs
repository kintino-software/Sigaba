using Sigaba.Primitives;

namespace Sigaba.App.Services.SigabaFiles;

internal record SigabaFileSaveResult(FilePath OutputPath);
internal record SigabaFileLoadResult(ISigabaFile SigabaFile, FilePath SigabaFilePath);

internal interface ISigabaFileManager
{
    Task<SigabaFileSaveResult> SaveAsync(ISigabaFile sigabaFile, DirPath projectRoot);
    Task<SigabaFileLoadResult> LoadAsync(DirPath referenceFolder);
    ISigabaFile CreateDefault(PublicKey publicKey);
}
