using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sigaba.App.DependencyInjection;
using Sigaba.Cli.Commands.Decrypt;
using Sigaba.Cli.Commands.Edit;
using Sigaba.Cli.Commands.Encrypt;
using Sigaba.Cli.Commands.Init;
using Sigaba.Cli.DependencyInjection;
using Sigaba.Cli.Models;
using Spectre.Console.Cli;
using System.Reflection;

namespace Sigaba.Cli;

internal static class AnsiConsoleSetup
{
    public static ITypeRegistrar CreateTypeRegistrar(IGlobalOptions globalOptions, Action<IServiceCollection>? config = null)
    {
        var services = new ServiceCollection();
        services.AddApp();
        services.AddCliApp(globalOptions);
        config?.Invoke(services);
        return new SpectreTypeRegistrar(services);
    }

    public static void Configure(this IConfigurator config, Action<IConfigurator>? additionalConfig = null)
    {
        config.SetApplicationName("sigaba");

        config.SetApplicationVersion(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");

        config.SetHelpProvider(new CustomSepctreHelpProvider(config.Settings));

        config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
        config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts all files defined in the configuration.");
        config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts all files defined in the configuration.");
        config.AddCommand<EditCommand>("edit").WithDescription("Opens the specified file and encrypts it after editing.");

        config.SetExceptionHandler(ExceptionHandler);

        additionalConfig?.Invoke(config);
    }

    private static int ExceptionHandler(Exception ex, ITypeResolver? resolver)
    {
        var logger = (resolver?.Resolve(typeof(ILogger<Exception>)) as ILogger)
            ?? throw new ArgumentNullException(nameof(resolver));

        logger.LogError("Error: {message}", ex.Message);

        return -1;
    }
}
