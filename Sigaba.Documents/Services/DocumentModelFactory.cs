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
        if (filePath.GetFileName().StartsWith(".env"))
        {
            return new EnvDocumentModel();
        }

        var extensionWithDot = filePath.ExtensionWithDot.ToLower();
        return extensionWithDot switch
        {
            ".json" => new JsonDocumentModel(),
            _ => throw new NotSupportedException($"File extension '{extensionWithDot}' is not supported.")
        };
    }
}
