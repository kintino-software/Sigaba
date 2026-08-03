namespace Kintino.CipherConf.App;

public interface IContextLoader
{
    Task CreateContextAsync(string folderPath);
    Task<IContext?> LoadContextAsync(string folderPath);
}