using Kintino.CipherConf.App.Dependencies;

namespace Kintino.CipherConf.Cli.TestHelpers;

public class TextEditorMock : ITextEditor
{
    public ValueTask EditFile(string filePath)
    {
        return ValueTask.CompletedTask;
    }
}
