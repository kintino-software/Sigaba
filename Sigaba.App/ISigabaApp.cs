namespace Sigaba.App;

public record InitializationOptions
{
    public required string SigabaFileOutputDir { get; init; }
    public required string PrivateKeyPassword { get; init; }
}

public interface ISigabaApp
{
    Task InitAsync(InitializationOptions options);
    Task CipherFilesAsync(string referenceFolderPath);
    Task DecipherFilesAsync(string referenceFolderPath, string password);
    Task EditFileAsync(ITextEditor textEditor, string editingFilePath);
}
