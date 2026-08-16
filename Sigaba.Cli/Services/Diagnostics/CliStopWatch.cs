using Spectre.Console;

namespace Sigaba.Cli.Services.Diagnostics;

internal class CliStopWatch(IAnsiConsole console)
{
    public async Task<T> MeasureAsync<T>(Func<Task<T>> action)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await action();
        stopwatch.Stop();
        console.WriteDefaultLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms.");
        return result;
    }
}
