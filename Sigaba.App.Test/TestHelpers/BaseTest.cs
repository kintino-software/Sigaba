using Microsoft.Extensions.Logging;

namespace Sigaba.App.TestHelpers;

[Collection(nameof(Fixture))]
public abstract class BaseTest
{
    public static ILogger<T> CreateLogger<T>() => Substitute.For<ILogger<T>>();
}
