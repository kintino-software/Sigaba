using Kintino.CipherConf.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {

        // internal
        services
            .AddSingleton<IEncryptConfigApp, EncryptConfigApp>();

        return services;
    }
}
