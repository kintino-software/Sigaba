using Microsoft.Extensions.DependencyInjection;

namespace Sigaba.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddAppPersistence(this IServiceCollection services)
    {
        //services
        //    .AddSingleton<IContextLoader, ContextLoader>()
        //    .AddSingleton<IToolSettingsManager, ToolSettingsManager>()
        //    .AddSingleton<IPublicKeyManager, PublicKeyManager>()
        //    .AddSingleton<IPrivateKeyManager, PrivateKeyManager>();

        return services;
    }
}
