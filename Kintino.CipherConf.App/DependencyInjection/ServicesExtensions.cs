using Kintino.CipherConf.Crypto.DependencyInjection;
using Kintino.CipherConf.Documents.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services, IFileSystem? fs = null)
    {
        services
            .AddSingleton<IFileSystem>(fs ?? new FileSystem())
            .AddCryptoModule()
            .AddDocumentsModule()
            .AddAppPersistence();

        return services;
    }
}
