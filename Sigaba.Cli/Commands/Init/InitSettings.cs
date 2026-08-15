using Spectre.Console.Cli;
using System.ComponentModel;

namespace Sigaba.Cli.Commands.Init;

public class InitSettings : CommandSettings
{
    [CommandOption("-n|--non-interactive")]
    [Description("Runs the command in non-interactive mode.")]
    public bool NonInteractive { get; set; } = false;

    [CommandOption("-p|--password <PASSWORD>")]
    [Description("Sets the password to decrypt the private key.")]
    public string Password { get; set; } = string.Empty;

    [CommandOption("-o|--output-private-key <PASSWORD>")]
    [Description("Sets the output directory of the private key.")]
    public string PrivateKeyOutputDir { get; set; } = string.Empty;
}
