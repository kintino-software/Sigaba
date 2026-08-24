using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics;

namespace Sigaba.Cli.Adaptors;

internal class CommandInterceptor : ICommandInterceptor
{
    private readonly Stopwatch sw = new();
    void ICommandInterceptor.Intercept(CommandContext context, CommandSettings settings)
    {
        sw.Restart();
    }

    void ICommandInterceptor.InterceptResult(CommandContext context, CommandSettings settings, ref int result)
    {
        sw.Stop();
        AnsiConsole.MarkupLine($"[gray]Execution time: {sw.ElapsedMilliseconds} ms[/]");
    }
}
