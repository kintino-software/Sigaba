using System.IO.Abstractions.TestingHelpers;

namespace Kintino.CipherConf.App.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
}
