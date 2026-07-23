using Kintino.CipherConf.Documents.Models;
using Kintino.CipherConf.Documents.Services.Json;

namespace Kintino.CipherConf.Documents.Services;

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
