using Microsoft.Extensions.Logging;

namespace Sigaba.Cli.Services.Diagnostics;

internal class CliStopWatch(ILogger<CliStopWatch> logger)
{
    public async Task<T> MeasureAsync<T>(Func<Task<T>> action)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await action();
        stopwatch.Stop();
        logger.LogInformation("Execution time: {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
        return result;
    }
}
