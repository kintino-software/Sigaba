using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

namespace Sigaba.Cli.Adaptors;

internal class CustomSepctreHelpProvider : HelpProvider
{
    public CustomSepctreHelpProvider(ICommandAppSettings settings) : base(settings)
    {
        settings.ShowOptionDefaultValues = false;
    }
}
