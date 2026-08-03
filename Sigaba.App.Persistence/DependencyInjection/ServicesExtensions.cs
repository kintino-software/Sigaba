using Microsoft.Extensions.DependencyInjection;
using Sigaba.App.Services.Contexts;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.PublicKeys;
using Sigaba.App.Services.Settings;

namespace Sigaba.App.DependencyInjection;

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
