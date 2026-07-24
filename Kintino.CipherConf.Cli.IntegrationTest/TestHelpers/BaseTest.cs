using Kintino.CipherConf.App.Dependencies;
using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.Cli.TestHelpers;

public abstract class BaseTest
{
    public const string ConfigFileName = "config.json";
    public const string PrivateKeyFileName = "private.key";
    public const string PublicKeyFileName = "public.key";

    protected static string RootPath { get; } = OperatingSystem.IsWindows() ? @"C:\" : "/";
    protected MockFileSystem Fs { get; } = new();
    protected ITextEditor TextEditor { get; } = Substitute.For<ITextEditor>();

    protected BaseTest()
    {
        Fs.Directory.SetCurrentDirectory(RootPath);
    }

    protected CliApp CreateApp()
    {
        return new CliApp(config =>
        {
            config.FileSystem = Fs;
            config.ToolSettingsFileName = ConfigFileName;
            config.PrivateKeyFileName = PrivateKeyFileName;
            config.PublicKeyFileName = PublicKeyFileName;
        });
    }

}
