using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Kintino.CipherConf.Documents.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddDocumentsModule(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDocumentModel, JsonDocumentModel>()
            .AddSingleton<IFileCipher, FileCipher>();
    }
}
