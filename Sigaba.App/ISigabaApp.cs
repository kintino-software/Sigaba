namespace Sigaba.App;

public interface ISigabaApp
{
    Task InitAsync(string initializationFolderPath);
    Task CipherFilesAsync(string referenceFolderPath);
    Task DecipherFilesAsync(string referenceFolderPath);
    Task EditFileAsync(ITextEditor textEditor, string editingFilePath);
}
