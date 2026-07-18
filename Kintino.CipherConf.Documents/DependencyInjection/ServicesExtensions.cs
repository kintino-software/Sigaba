using Kintino.CipherConf.Documents.Implementations;
using Kintino.CipherConf.Documents.Services;
using Kintino.CipherConf.Documents.Services.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Documents.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddDocumentsModule(this IServiceCollection services)
    {
        return services
            .AddSingleton<IValueCipher, ValueCipher>()
            .AddSingleton<IFileCipher, FileCipher>()
            .AddSingleton<ICipherResolver, CipherResolver>()
            .AddSingleton<IDocumentCipher, JsonDocumentCipher>();
        ;
    }
}
