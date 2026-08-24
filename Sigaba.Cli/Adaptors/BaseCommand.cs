using Sigaba.Cli.Models;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Adaptors;

internal abstract class BaseCommand<T>(IGlobalOptions globalOptions) : AsyncCommand<T> where T : BaseCommandSettings
{
    protected override async Task<int> ExecuteAsync(CommandContext context, T settings, CancellationToken cancellationToken)
    {
        globalOptions.SetVerbosity(
            settings.IsQuiet
            ? VerbosityLevel.Quiet
            : settings.Verbosity);

        return await ExecuteCoreAsync(context, settings, cancellationToken);
    }

    protected abstract Task<int> ExecuteCoreAsync(CommandContext context, T settings, CancellationToken cancellationToken);
}

internal abstract class BaseCommand(IGlobalOptions globalOptions) : BaseCommand<BaseCommandSettings>(globalOptions);