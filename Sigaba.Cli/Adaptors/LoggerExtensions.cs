using Microsoft.Extensions.Logging;
using Sigaba.App;
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

    extension(ILogger logger)
    {
        public void LogCipherResult(LogLevel level, CipherResult result)
        {
            string[] files = [.. result.PathsOfFilesAffected];
            if (files.Length == 0)
            {
                logger.Log(level, "No files were affected.");
                return;
            }

            logger.Log(level, "{count} file(s) affected:", result.PathsOfFilesAffected.Count());
            foreach (var file in result.PathsOfFilesAffected)
            {
                logger.Log(level, "  {file}", file);
            }
        }
    }
}
