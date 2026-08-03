using Sigaba.Documents.Models;
using Sigaba.Documents.Services.Json;

namespace Sigaba.Documents.Services;

internal static class DocumentModelFactory
{
    public static IDocumentModel GetDocumentModelByFileExtension(string extensionWithDot)
    {
        extensionWithDot = extensionWithDot.ToLower();

        return extensionWithDot switch
        {
            ".json" => new JsonDocumentModel(),
            _ => throw new NotSupportedException($"File extension '{extensionWithDot}' is not supported.")
        };
    }
}
