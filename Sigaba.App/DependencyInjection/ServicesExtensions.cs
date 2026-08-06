using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.Services;
using Sigaba.App.Services.Contexts;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.PublicKeys;
using Sigaba.App.Services.Settings;
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
            .AddSingleton<IContextLoader, ContextLoader>()
            .AddSingleton<IToolSettingsManager, ToolSettingsManager>()
            .AddSingleton<IPublicKeyManager, PublicKeyManager>()
            .AddSingleton<IPrivateKeyManager, PrivateKeyManager>()
            .AddSingleton<ISigabaApp, SigabaApp>();

        return services;
    }
}
