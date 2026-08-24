using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sigaba.Cli.Commands.Decrypt;
using Sigaba.Cli.Commands.Edit;
using Sigaba.Cli.Commands.Encrypt;
using Sigaba.Cli.Commands.Init;
using Sigaba.Cli.DependencyInjection;
using Spectre.Console.Cli;

namespace Sigaba.Cli.Adaptors;

internal static class AnsiConsoleSetup
{
    public static ITypeRegistrar CreateTypeRegistrar(Action<IServiceCollection>? config = null)
    {
        var services = new ServiceCollection();
        services.AddCliApp();
        config?.Invoke(services);
        return new SpectreTypeRegistrar(services);
    }

    public static void Configure(this IConfigurator config, Action<IConfigurator>? additionalConfig = null)
    {
        config.AddCommand<InitCommand>("init").WithDescription("Sets up the initial configuration.");
        config.AddCommand<EncryptCommand>("encrypt").WithDescription("Encrypts all files defined in the configuration.");
        config.AddCommand<DecryptCommand>("decrypt").WithDescription("Decrypts all files defined in the configuration.");
        config.AddCommand<EditCommand>("edit").WithDescription("Opens the specified file and encrypts it after editing.");

        config.SetApplicationName("sigaba");
        config.UseAssemblyInformationalVersion();
        config.SetExceptionHandler(ExceptionHandler);
        config.SetInterceptor(new CommandInterceptor());

        additionalConfig?.Invoke(config);
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
