using Kintino.CipherConf.App.Services.Contexts;
using Kintino.CipherConf.App.Services.PrivateKeys;
using Kintino.CipherConf.App.Services.PublicKeys;
using Kintino.CipherConf.App.Services.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddAppPersistence(this IServiceCollection services)
    {
        services
            .AddSingleton<IContextLoader, ContextLoader>()
            .AddSingleton<IToolSettingsRepository, ToolSettingsFileRepository>()
            .AddSingleton<IPublicKeyRepository, PublicKeyFileRepository>()
            .AddSingleton<IPrivateKeyRepository, PrivateKeyFileRepository>();

        return services;
    }
}
