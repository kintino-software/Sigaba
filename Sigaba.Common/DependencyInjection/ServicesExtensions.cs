using Microsoft.Extensions.DependencyInjection;
using Sigaba.Services;
using System.IO.Abstractions;

namespace Sigaba.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddCommonModule(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IEnvironmentVariables, SystemEnvironmentVariables>();

        return services;
    }
}
