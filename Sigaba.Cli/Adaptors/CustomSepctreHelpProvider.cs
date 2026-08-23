using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

namespace Sigaba.Cli.Adaptors;

internal class CustomSepctreHelpProvider : HelpProvider
{
    public CustomSepctreHelpProvider(ICommandAppSettings settings) : base(settings)
    {
    }

    public override IEnumerable<IRenderable> Write(ICommandModel model, ICommandInfo? command)
    {


        return base.Write(model, command);
    }
}
