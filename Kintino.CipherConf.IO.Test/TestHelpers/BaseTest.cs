using Kintino.CipherConf.IO.Dependencies;

namespace Kintino.CipherConf.IO.TestHelpers;

public abstract class BaseTest
{
    protected IIOConfiguration Configuration { get; } = Substitute.For<IIOConfiguration>();
    protected MockFileSystem Fs { get; } = new();
    protected static string RootPath { get; }

    static BaseTest()
    {
        RootPath = OperatingSystem.IsWindows() ? @"C:\" : "/";
    }

}
