using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.Services.Contexts;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.SigabaFiles;
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
            .AddSingleton<ISigabaFileManager, SigabaFileManager>()
            .AddSingleton<IPrivateKeyManager, PrivateKeyManager>()
            .AddSingleton<IContextLoader, ContextLoader>()
            .AddSingleton<ISigabaApp, SigabaApp>();

        return services;
    }
}
