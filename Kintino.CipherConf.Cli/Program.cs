using Kintino.CipherConf.Cli.Services;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli;

internal class Program
{
    static async Task Main(string[] args)
    {
        var app = new App(fs: new FileSystem(), textEditor: new WindowsEditTextEditor());
        await app.RunAsync(args);
    }
}
