namespace Kintino.CipherConf.Models;

public interface IFieldFilter
{
    bool Match(string fieldName);
}
