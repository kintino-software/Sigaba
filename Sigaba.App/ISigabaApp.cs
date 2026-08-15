using Sigaba.Primitives;

namespace Sigaba.App;

public interface ISigabaApp
{
    Task<InitializationResult> InitAsync(InitializationOptions options);
    Task<CipherResult> CipherFilesAsync(DirPath referenceFolderPath);
    Task<CipherResult> DecipherFilesAsync(DirPath referenceFolderPath, string password);
    Task EditFileAsync(ITextEditor textEditor, FilePath editingFilePath);
}

public record InitializationOptions(DirPath SigabaFileOutputDir, string PrivateKeyPassword);

public record InitializationResult(FilePath SigabaFileLocation, FilePath PrivateKeyLocation);

public record CipherResult(IEnumerable<string> PathsOfFilesAffected);
