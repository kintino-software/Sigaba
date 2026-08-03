using Spectre.Console.Cli;
using System.ComponentModel;

namespace Sigaba.Cli.Adaptors.SpectreConsole;

internal class GlobalSettings : CommandSettings
{
    [CommandOption("-p|--project")]
    [Description("The path to the project folder.")]
    public string? ProjectDirPath { get; set; }
}
