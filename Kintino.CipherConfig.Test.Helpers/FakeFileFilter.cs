using Kintino.CipherConf.Models;

namespace Kintino.CipherConfig;

public class FakeFileFilter : IFileFilter
{
    private Func<string, bool> matchFunc = (_) => true;

    public bool Match(string fileFullPath)
    {
        return matchFunc(fileFullPath);
    }

    public FakeFileFilter SetMatchFunc(Func<string, bool> matchFunc)
    {
        this.matchFunc = matchFunc;
        return this;
    }
}
