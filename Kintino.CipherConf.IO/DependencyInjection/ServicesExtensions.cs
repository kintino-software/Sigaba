using Kintino.CipherConf.IO.Dependencies;
using Kintino.CipherConf.IO.Implementations;
using Kintino.CipherConf.IO.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.IO.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddECFileSystem(this IServiceCollection services, IOConfiguration configuration)
    {
        return services
            .AddSingleton<IIOConfiguration>(configuration)
            .AddSingleton<IContextRepository, ContextRepository>()
            .AddSingleton<IDataSerializer, DataSerializer>()
            .AddSingleton<IFileOperations, FileOperations>();
    }
}
