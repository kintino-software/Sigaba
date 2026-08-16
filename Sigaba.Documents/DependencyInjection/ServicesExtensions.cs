using Microsoft.Extensions.DependencyInjection;
using Sigaba.Documents.Models;
using Sigaba.Documents.Services.Json;

namespace Sigaba.Documents.DependencyInjection;

public static class ServicesExtensions
{
  public static IServiceCollection AddDocumentsModule(this IServiceCollection services)
  {
    return services
        .AddSingleton<IDocumentModel, JsonDocumentModel>()
        .AddSingleton<IFileCipher, FileCipher>();
  }
}
