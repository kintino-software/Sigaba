using Kintino.CipherConf.App.Services;
using Kintino.CipherConf.Crypto.DependencyInjection;
using Kintino.CipherConf.Documents.DependencyInjection;
using Kintino.CipherConf.IO.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services, AppConfiguration configuration, DependencyFactory dependencyFactory)
    {
        // modules
        services
            .AddCryptoModule()
            .AddIOModule(configuration)
            .AddDocumentsModule();

        // dependencies
        services
            .AddSingleton(sp => dependencyFactory.TextEditorFactory(sp))
            .AddSingleton(sp => dependencyFactory.FileSystemFactory(sp));

        // internal
        services
            .AddSingleton<IEncryptConfigApp, EncryptConfigApp>();

        return services;
    }
}
