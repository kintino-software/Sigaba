using Sigaba.Cli.Models;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sigaba.Cli.Adaptors;

internal class BaseCommandSettings : CommandSettings
{
    [CommandOption("-q|--quiet")]
    [Description("(Optional) No console output. Same as verbosity level Quiet.")]
    public bool IsQuiet { get; set; } = false;

    [CommandOption("--verbosity")]
    [AllowedValues(VerbosityLevel.Normal, VerbosityLevel.Detailed, VerbosityLevel.Quiet)]
    [Description("(Optional) Sets the verbosity level (Detailed, Normal, Quiet). Default: Normal")]
    public VerbosityLevel Verbosity { get; set; } = VerbosityLevel.Normal;

}

