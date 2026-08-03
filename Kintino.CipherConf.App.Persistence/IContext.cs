using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App;

public interface IContext
{
    PrivateKey? GetPrivateKey();
    PublicKey? GetPublicKey();
    bool FieldNameFilter(string fieldName);
    IEnumerable<string> GetWorkingSetFiles();
}

