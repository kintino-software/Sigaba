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
                VerbosityLevel.Diagnostic => LogLevel.Trace,
                VerbosityLevel.Detailed => LogLevel.Debug,
                VerbosityLevel.Normal => LogLevel.Information,
                VerbosityLevel.Minimal => LogLevel.Warning,
                _ => LogLevel.Information,
            };
        }
    }
}
