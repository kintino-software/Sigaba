namespace Kintino.CipherConf.App;

public interface IContextLoader
{
    Task<bool> HasContextAsync(string folderPath);
    Task CreateContextAsync(string folderPath);
    Task<IContext?> LoadContextAsync(string folderPath);
}