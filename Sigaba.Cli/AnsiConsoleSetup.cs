using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sigaba.App.DependencyInjection;
using Sigaba.Cli.Commands.Decrypt;
using Sigaba.Cli.Commands.Edit;
using Sigaba.Cli.Commands.Encrypt;
using Sigaba.Cli.Commands.Init;
using Sigaba.Cli.DependencyInjection;
using Spectre.Console.Cli;

namespace Sigaba.Cli;

public class AnsiConsoleSetup
{
    public static ITypeRegistrar CreateTypeRegistrar(Action<IServiceCollection>? config = null)
    {
        var services = new ServiceCollection();
        services.AddApp();
        services.AddCliApp();
        config?.Invoke(services);
        return new SpectreTypeRegistrar(services);
    }

    public static void Configurator(IConfigurator config, Action<IConfigurator>? additionalConfig = null)
    {
        config.SetApplicationName("sigaba");
        config.SetExceptionHandler(ExceptionHandler);
        config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
        config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts all files defined in the configuration.");
        config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts all files defined in the configuration.");
        config.AddCommand<EditCommand>("edit").WithDescription("Opens the specified file and encrypts it after editing.");
        additionalConfig?.Invoke(config);
    }

    private static int ExceptionHandler(Exception ex, ITypeResolver? resolver)
    {
        var logger = (resolver?.Resolve(typeof(ILogger<AnsiConsoleSetup>)) as ILogger<AnsiConsoleSetup>)
            ?? throw new ArgumentNullException(nameof(resolver));

        logger.LogError("Error: {message}", ex.Message);

        return -1;
    }
}
