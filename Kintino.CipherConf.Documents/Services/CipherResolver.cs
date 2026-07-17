using Kintino.CipherConf.Documents.Services.Json;
using System.IO.Abstractions;

namespace Kintino.CipherConf.Documents.Services;

internal class CipherResolver(IEnumerable<IDocumentCipher> documentCiphers, IFileSystem fs) : ICipherResolver
{
    public IDocumentCipher Resolve(string filePath)
    {
        var extension = fs.Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".json" => documentCiphers.FirstOrDefault(c => c is JsonDocumentCipher)
                        ?? throw new InvalidOperationException("No JSON cipher found."),
            _ => throw new InvalidOperationException($"File extension '{extension}' is not supported."),
        };
    }
}
