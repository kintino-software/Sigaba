using Kintino.CipherConf.Tooling;

namespace Kintino.CipherConfig;

public class FakeTextEditor : ITextEditor
{
    public ValueTask EditFile(string filePath)
    {
        return ValueTask.CompletedTask;
    }
}
