using Sigaba.Primitives;

namespace Sigaba.App;

public record InitializationOptions
{
    public required DirPath SigabaFileOutputDir { get; init; }
    public required string PrivateKeyPassword { get; init; }
}

public interface ISigabaApp
{
    Task InitAsync(InitializationOptions options);
    Task CipherFilesAsync(DirPath referenceFolderPath);
    Task DecipherFilesAsync(DirPath referenceFolderPath, string password);
    Task EditFileAsync(ITextEditor textEditor, FilePath editingFilePath);
}
