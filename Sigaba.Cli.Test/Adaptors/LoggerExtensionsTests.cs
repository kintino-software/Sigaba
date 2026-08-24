using Microsoft.Extensions.Logging;
using Sigaba.Cli.Models;

namespace Sigaba.Cli.Adaptors;

public class LoggerExtensionsTests
{
    [Fact]
    public void Should_convert_verbosity_level_to_log_level()
    {
        Dictionary<VerbosityLevel, LogLevel> pairs = new()
        {
            [VerbosityLevel.Detailed] = LogLevel.Debug,
            [VerbosityLevel.Normal] = LogLevel.Information,
            [VerbosityLevel.Quiet] = LogLevel.None
        };
        pairs.Keys.Should().BeEquivalentTo(Enum.GetValues<VerbosityLevel>(), "all verbosity levels should be checked");

        foreach (var (verbosity, logLevel) in pairs)
        {
            verbosity.ToLogLevel().Should().Be(logLevel);
        }

    }
}

