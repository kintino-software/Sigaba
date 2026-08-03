using Sigaba.Primitives;

namespace Sigaba.App;

public interface IContext
{
    PrivateKey? GetPrivateKey();
    PublicKey? GetPublicKey();
    bool FieldNameFilter(string fieldName);
    IEnumerable<string> GetWorkingSetFiles();
}

