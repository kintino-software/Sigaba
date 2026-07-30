namespace Kintino.CipherConf.App;

public interface IEncryptConfigApp
{
    Task InitAsync();
    Task CipherFilesAsync();
    Task DecipherFilesAsync();
    Task EditFileAsync(ITextEditor textEditor, string editingFilePath);
}
