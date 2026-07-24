using Kintino.CipherConf.App.DependencyInjection;
using Kintino.CipherConf.Crypto.DependencyInjection;
using Kintino.CipherConf.Documents.DependencyInjection;
using Kintino.CipherConf.IO.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO.Abstractions;

namespace Kintino.CipherConf.DependencyInjection;

public static class ServicesExtensions
{
    public static void AddCipherConfServices(
        this IServiceCollection services,
        Action<AppConfiguration>? configure = null
        )
    {
        // configuration

        var appConfiguration = new AppConfiguration();
        configure?.Invoke(appConfiguration);

        services.RemoveAll<IFileSystem>(); // guarantee that we have only one IFileSystem
        services.AddSingleton<IFileSystem>(appConfiguration.FileSystem);

        // modules
        services
            .AddApp()
            .AddCryptoModule()
            .AddIOModule(appConfiguration)
            .AddDocumentsModule();
    }
}
