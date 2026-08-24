using Sigaba.Cli.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sigaba.Cli.Commands;

internal class BaseCommandSettings : CommandSettings
{
    [CommandOption("-q|--quiet")]
    [Description("No console output. Same as verbosity level Quiet.")]
    public bool IsQuiet { get; set; } = false;

    [CommandOption("--verbosity")]
    [AllowedValues(VerbosityLevel.Normal, VerbosityLevel.Detailed, VerbosityLevel.Quiet)]
    [DefaultValue(VerbosityLevel.Normal)]
    [Description("Sets the verbosity level.")]
    public VerbosityLevel Verbosity { get; set; } = VerbosityLevel.Normal;

}
