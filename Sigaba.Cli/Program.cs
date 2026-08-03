namespace Sigaba.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        var app = new CliApp();
        await app.RunAsync(args);
    }
}
