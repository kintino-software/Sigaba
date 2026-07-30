using Kintino.CipherConf.App;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Cli.TestHelpers;

public abstract class BaseTest
{
    protected static string RootPath { get; } = OperatingSystem.IsWindows() ? @"C:\" : "/";
    protected MockFileSystem Fs { get; } = new();
    protected ITextEditor TextEditor { get; } = Substitute.For<ITextEditor>();

    protected BaseTest()
    {
        Fs.Directory.SetCurrentDirectory(RootPath);
    }

    protected CliApp CreateApp()
    {
        return new CliApp(Fs);
    }

}
