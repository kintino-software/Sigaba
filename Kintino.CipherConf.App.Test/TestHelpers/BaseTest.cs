using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.App.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
    protected string RootDir { get; }

    protected BaseTest()
    {
        RootDir = Fs.Path.GetPathRoot(Fs.AllPaths.First());
    }
}
