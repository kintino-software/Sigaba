namespace Sigaba.App;

public interface ISigabaApp
{
    Task InitAsync();
    Task CipherFilesAsync();
    Task DecipherFilesAsync();
    Task EditFileAsync(ITextEditor textEditor, string editingFilePath);
}
