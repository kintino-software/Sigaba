using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
using Sigaba.Crypto.DependencyInjection;
using Sigaba.DependencyInjection;
using Sigaba.Documents.DependencyInjection;

namespace Sigaba.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        // as we want this assembly to be self-contained, we will register the modules here instead of in the main program
        // so ui implementations dont have to know about the modules, they just need to call AddApp and everything will be registered

        // modules
        services
            .AddCommonModule()
            .AddCryptoModule()
            .AddDocumentsModule();

        // local
        services
            .AddSingleton<ISigabaFileManager, SigabaFileManager>()
            .AddSingleton<IPrivateKeyManager, PrivateKeyManager>()
            .AddSingleton<ISigabaApp, SigabaApp>();

        return services;
    }
}
