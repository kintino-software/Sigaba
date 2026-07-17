namespace Kintino.CipherConf.Models;

public interface IFileFilter
{
    bool Match(string fileFullPath);
}
