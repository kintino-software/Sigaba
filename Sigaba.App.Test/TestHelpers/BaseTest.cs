using Microsoft.Extensions.Logging;
using System.IO.Abstractions.TestingHelpers;

namespace Sigaba.App.TestHelpers;

[Collection(nameof(Fixture))]
public abstract class BaseTest
{
    protected MockFileSystem Fs { get; } = new();

    public static ILogger<T> CreateLogger<T>() => Substitute.For<ILogger<T>>();
}
