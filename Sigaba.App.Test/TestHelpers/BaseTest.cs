using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.App.TestHelpers;

[Collection(nameof(Fixture))]
public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();
}
