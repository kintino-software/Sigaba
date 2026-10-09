using Sigaba.Documents.Models;
using Sigaba.Documents.Services.Env;
using Sigaba.Documents.Services.Json;
using Sigaba.Primitives.FileSystem;

namespace Sigaba.Documents.Services;

internal static class DocumentModelFactory
{
    public static IDocumentModel GetDocumentModelByFilePath(FilePath filePath)
    {
        // env files could be like: .env, .env.local, .env.development, etc.
        if (Path.GetFileName(filePath).StartsWith(".env"))
        {
            return new EnvDocumentModel();
        }

        var extensionWithDot = Path.GetExtension(filePath).ToLower();
        return extensionWithDot switch
        {
            ".json" => new JsonDocumentModel(),
            _ => throw new NotSupportedException($"File extension '{extensionWithDot}' is not supported.")
        };
    }
}
