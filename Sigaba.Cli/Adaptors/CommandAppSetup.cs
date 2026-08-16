using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.DependencyInjection;
using Sigaba.Cli.Commands;
using Sigaba.Cli.Commands.Init;
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
    config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts all files defined in the configuration.");
    config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts all files defined in the configuration.");
    config.AddCommand<EditCommand>("edit").WithDescription("Opens the specified file and encrypts it after editing.");
    additionalConfig?.Invoke(config);
  }

}
