using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sigaba.Cli.Commands.Decrypt;
using Sigaba.Cli.Commands.Edit;
using Sigaba.Cli.Commands.Encrypt;
using Sigaba.Cli.Commands.Init;
using Sigaba.Cli.DependencyInjection;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Adaptors;

internal class AnsiConsoleSetup
{
    public ITypeRegistrar TypeRegistrar { get; }
    public Action<IConfigurator> Configurator { get; }

    private AnsiConsoleSetup(ITypeRegistrar typeRegistrar, Action<IConfigurator> configuratior)
    {
        TypeRegistrar = typeRegistrar;
        Configurator = configuratior;
    }

    public static AnsiConsoleSetup Create(Action<IServiceCollection>? additionalServicesConfig = null)
    {
        var services = new ServiceCollection();
        services.AddCliApp();
        additionalServicesConfig?.Invoke(services);

        var typeRegistrar = new SpectreTypeRegistrar(services);
        return new AnsiConsoleSetup(
            typeRegistrar,
            Configure);

    }

    private static void Configure(IConfigurator config)
    {
        config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
        config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts all files defined in the configuration.");
        config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts all files defined in the configuration.");
        config.AddCommand<EditCommand>("edit").WithDescription("Opens the specified file and encrypts it after editing.");

        config.SetApplicationName("sigaba");
        config.UseAssemblyInformationalVersion();
        config.SetExceptionHandler(ExceptionHandler);
        config.SetInterceptor(new CommandInterceptor());
    }

    private static int ExceptionHandler(Exception ex, ITypeResolver? resolver)
    {
        var logger = (resolver?.Resolve(typeof(ILogger<CommandApp>)) as ILogger);

        // some early exceptions may occur before the logger is registered, so we need to handle that case
        if (logger == null)
        {
            Console.WriteLine("Error: {0}", ex.Message);
        }
        else
        {
            logger.LogError("Error: {message}", ex.Message);
        }

        return -1;
    }
}
