using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.Service;
using Sigaba.Crypto.DependencyInjection;
using Sigaba.Documents.DependencyInjection;
using System.IO.Abstractions;

namespace Sigaba.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services, IFileSystem? fs = null)
    {
        services
            .AddSingleton<IFileSystem>(fs ?? new FileSystem())
            .AddCryptoModule()
            .AddDocumentsModule()
            .AddAppPersistence()
            .AddSingleton<IFsHelper, FsHelper>()
            .AddSingleton<ISigabaApp, SigabaApp>();

        return services;
    }
}
