using Kintino.CipherConf.App.Services;
using Kintino.CipherConf.Cli.Adaptors.SpectreConsole;
using Spectre.Console.Cli;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Cli.Commands;

internal class EncryptCommand(IECApp ecapp, IFileSystem fs) : CommandWithGlobalSettings
{
    protected override Task<int> ExecuteAsync(CommandContext context, GlobalSettings settings, CancellationToken cancellationToken)
    {
        return TryRunAsync(() => ecapp.CipherFiles(GetProjectTargetDir(settings, fs)));
    }
}
