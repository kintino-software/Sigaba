using Kintino.CipherConf.IO.Dependencies;
using Kintino.CipherConf.IO.Implementations;
using Kintino.CipherConf.IO.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.IO.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddIOModule(this IServiceCollection services, IIOConfiguration configuration)
    {
        return services
            .AddSingleton<IIOConfiguration>(configuration)
            .AddSingleton<IContextRepository, ContextRepository>()
            .AddSingleton<IDataSerializer, DataSerializer>()
            .AddSingleton<IFileOperations, FileOperations>();
    }
}
