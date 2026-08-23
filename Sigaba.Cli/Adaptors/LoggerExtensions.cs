using Microsoft.Extensions.Logging;
using Sigaba.Cli.Models;

namespace Sigaba.Cli.Adaptors;

internal static class LoggerExtensions
{
    extension(VerbosityLevel verbosity)
    {
        public LogLevel ToLogLevel()
        {
            return verbosity switch
            {
                VerbosityLevel.Quiet => LogLevel.None,
                VerbosityLevel.Normal => LogLevel.Information,
                VerbosityLevel.Detailed => LogLevel.Debug,
                _ => LogLevel.Information,
            };
        }
    }
}
