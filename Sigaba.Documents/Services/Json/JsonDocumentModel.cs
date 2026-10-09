using Sigaba.Documents.Models;
using Sigaba.Documents.Services.Json.Parser;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Sigaba.Documents.Services.Json;

internal class JsonDocumentModel : IDocumentModel
{
    private Dictionary<string, JsonToken> tokensMap = [];
    private readonly Dictionary<string, string> replacements = [];

    private string originalContent = string.Empty;

    void IDocumentModel.Parse(string documentContent)
    {
        originalContent = documentContent;
        var tokens = JsonParser.Parse(documentContent);
        tokensMap = tokens.ToDictionary(token => token.Path, token => token);
    }

    string IDocumentModel.Serialize()
    {
        var orderedTokens = tokensMap.Values.OrderByDescending(token => token.StartIndex);
        foreach (var token in orderedTokens)
        {
            if (replacements.TryGetValue(token.Path, out var replacement))
            {
                originalContent = originalContent
                    .Remove(token.StartIndex, token.Length)
                    .Insert(token.StartIndex, replacement);
            }
        }

        return originalContent;
    }

    IEnumerable<string> IDocumentModel.GetFieldNames()
    {
        return tokensMap.Keys;
    }

    string IDocumentModel.GetFieldRawValue(string key)
    {
        if (tokensMap.TryGetValue(key, out var token))
            return token.RawValue;
        throw new KeyNotFoundException($"Key not found: {key}");
    }

    bool IDocumentModel.TryGetValueAsString(string key, [NotNullWhen(true)] out string? value)
    {
        if (!tokensMap.TryGetValue(key, out var token))
        {
            value = null;
            return false;
        }

        if (token.Type == JsonDataType.String)
        {
            value = token.RawValue.Trim('"');
        }
        else
        {
            value = token.RawValue;
        }

        return true;
    }

    // modification

    void IDocumentModel.SetFieldValue<T>(string key, [MaybeNull] T value)
    {
        if (!tokensMap.ContainsKey(key))
            throw new KeyNotFoundException($"Key not found: {key}");

        var jsonValue = JsonSerializer.Serialize(value);
        replacements.Add(key, jsonValue);
    }

    void IDocumentModel.SetFieldRawValue(string key, string rawValue)
    {
        if (!tokensMap.ContainsKey(key))
            throw new KeyNotFoundException($"Key not found: {key}");

        replacements.Add(key, rawValue);
    }



}
