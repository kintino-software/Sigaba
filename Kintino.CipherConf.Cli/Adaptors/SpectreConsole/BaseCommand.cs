using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli.Adaptors.SpectreConsole;

internal abstract class CommandWithGlobalSettings<TGlobalSettings> : AsyncCommand<TGlobalSettings> where TGlobalSettings : GlobalSettings
{
    protected static async Task<int> TryRunAsync(Func<Task> operation)
    {
        await operation();
        return 0;
    }

    protected static string GetProjectTargetDir(TGlobalSettings globalOptions, IFileSystem fs)
    {
        var resolvedProjectDirPath = globalOptions.ProjectDirPath == null ?
            fs.Directory.GetCurrentDirectory() :
            fs.Path.Combine(fs.Directory.GetCurrentDirectory(), globalOptions.ProjectDirPath);

        return resolvedProjectDirPath;
    }
}

internal abstract class CommandWithGlobalSettings : CommandWithGlobalSettings<GlobalSettings>;
