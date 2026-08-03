using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.App.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
    protected readonly string RootPath;

    protected BaseTest()
    {
        RootPath = Fs.Path.GetPathRoot(Fs.Directory.GetCurrentDirectory());
    }

    protected string FromRoot(params string[] relativePath)
    {
        return Path.Combine([RootPath, .. relativePath]);
    }

}
