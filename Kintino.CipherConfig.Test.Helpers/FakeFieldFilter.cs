using Kintino.CipherConf.Models;

namespace Kintino.CipherConfig;

public class FakeFieldFilter : IFieldFilter
{
    private Func<string, bool> matchFunc = (_) => true;

    public bool Match(string fileFullPath)
    {
        return matchFunc(fileFullPath);
    }

    public FakeFieldFilter SetMatchFunc(Func<string, bool> matchFunc)
    {
        this.matchFunc = matchFunc;
        return this;
    }
}
