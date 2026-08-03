using Kintino.CipherConf.App.Dependencies;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.App.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
    protected readonly string RootPath;

    protected BaseTest()
    {
        RootPath = Fs.Path.GetPathRoot(Fs.Directory.GetCurrentDirectory());
        FS.Setup(Fs);
    }

    protected string FromRoot(params string[] relativePath)
    {
        return Path.Combine([RootPath, .. relativePath]);
    }

}
