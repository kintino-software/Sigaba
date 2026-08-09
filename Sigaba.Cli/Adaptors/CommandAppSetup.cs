using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.DependencyInjection;
using Sigaba.Cli.Commands;
using Sigaba.Cli.DependencyInjection;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Adaptors;

public static class CommandAppSetup
{
    public static ITypeRegistrar CreateTypeRegistrar(Action<IServiceCollection>? additionalServiceSetup = null)
    {
        var services = new ServiceCollection();
        services.AddApp();
        services.AddCliApp();
        additionalServiceSetup?.Invoke(services);
        return new SpectreTypeRegistrar(services);

    }

    public static void Configurator(IConfigurator config, Action<IConfigurator>? additionalConfig = null)
    {
        config.PropagateExceptions();
        config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
        config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts the specified configuration.");
        config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts the specified configuration.");
        config.AddCommand<EditCommand>("edit").WithDescription("Edits the specified configuration.");
        additionalConfig?.Invoke(config);
    }

}
