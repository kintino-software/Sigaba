using Kintino.CipherConf.Primitives;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

internal class JsonDocumentCipher(IValueCipher valueCipher) : IDocumentCipher
{
    private readonly JsonSerializerOptions options = new() { WriteIndented = true };

    string IDocumentCipher.Encrypt(PlainKey key, string jsonDocument, Predicate<string>? propertyNameFilter)
    {
        var rootNode = ParseOrThrow(jsonDocument);
        var entriesToEncrypt = JsonTraverser.Traverse(rootNode)
            .Where(n => ShouldEncrypt(n.Key, propertyNameFilter))
            .ToArray();

        foreach (var entry in entriesToEncrypt)
        {
            var encryptedNode = valueCipher.CreateEncryptedValue(entry.Node, key);
            entry.Node.ReplaceWith(encryptedNode);
        }
        return rootNode.ToJsonString(options);
    }

    string IDocumentCipher.Decrypt(PlainKey key, string jsonDocument)
    {
        var rootNode = ParseOrThrow(jsonDocument);
        var entriesToDecrypt = JsonTraverser.Traverse(rootNode).ToArray();

        foreach (var entry in entriesToDecrypt)
        {
            var plainNode = valueCipher.CreateDecryptedValue(entry.Node, key);
            // as value cipher can return the same node instance, we need to check if the node is different before replacing it
            // condition avoids replacing the node with itself, which would cause an exception
            if (plainNode != entry.Node)
            {
                entry.Node.ReplaceWith(plainNode);
            }
        }
        return rootNode.ToJsonString(options);
    }

    // helper methods

    private static bool ShouldEncrypt(string? key, Predicate<string>? propertyNameFilter)
    {
        return key != null && (propertyNameFilter == null || propertyNameFilter(key));
    }

    private static JsonNode ParseOrThrow(string jsonDocument)
    {
        var rootNode = JsonNode.Parse(jsonDocument)
            ?? throw new InvalidOperationException("Invalid JSON document");
        return rootNode;
    }
}

