using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.App.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddECApp(this IServiceCollection services, ITextEditor textEditor)
    {
        return services
            .AddSingleton<IFacade, Facade>()
            .AddSingleton<IECApp, ECApp>()
            .AddSingleton<ITextEditor>(textEditor);
    }
}
