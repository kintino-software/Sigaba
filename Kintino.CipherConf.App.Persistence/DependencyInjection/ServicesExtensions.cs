using Kintino.CipherConf.App.Services;
using Kintino.CipherConf.App.Services.Contexts;
using Kintino.CipherConf.App.Services.PrivateKeys;
using Microsoft.Extensions.DependencyInjection;
using Kintino.CipherConf.App.Services.PublicKeys;
using Kintino.CipherConf.App.Services.Settings;

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
