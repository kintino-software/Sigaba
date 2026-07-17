using Kintino.CipherConf.App.Implementations;
using Kintino.CipherConf.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddECApp(this IServiceCollection services)
    {
        return services
            .AddSingleton<IFacade, Facade>()
            .AddSingleton<IEncryptConfigApp, EncryptConfigApp>();
    }
}
