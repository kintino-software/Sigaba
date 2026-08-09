using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.App.TestHelpers;

public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();

}
