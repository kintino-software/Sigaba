using Kintino.CipherConf.App.Services.Contexts;
using Kintino.CipherConf.App.Services.PrivateKeys;
using Kintino.CipherConf.App.Services.PublicKeys;
using Kintino.CipherConf.App.Services.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddAppPersistence(this IServiceCollection services, IFileSystem? fs = null)
    {
        if (fs != null)
            FS.Setup(fs);

        services
            .AddSingleton<IContextLoader, ContextLoader>()
            .AddSingleton<IToolSettingsRepository, ToolSettingsFileRepository>()
            .AddSingleton<IPublicKeyRepository, PublicKeyFileRepository>()
            .AddSingleton<IPrivateKeyRepository, PrivateKeyFileRepository>();

        return services;
    }
}
