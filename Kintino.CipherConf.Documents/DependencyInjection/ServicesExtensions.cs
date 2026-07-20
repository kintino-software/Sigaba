using Kintino.CipherConf.Documents.Implementations;
using Kintino.CipherConf.Documents.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Documents.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddDocumentsModule(this IServiceCollection services)
    {
        return services
            .AddSingleton<DocumentCipher>()
            .AddSingleton<IFileCipher, FileCipher>();
    }
}
