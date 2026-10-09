using Sigaba.Documents.Models;
using Sigaba.Documents.Services.Env.Parser;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sigaba.Documents.Services.Env;

internal class EnvDocumentModel() : IDocumentModel
{
    private IReadOnlyDictionary<string, EnvEntry> fields = new Dictionary<string, EnvEntry>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> replacements = [];
    private string originalContent = string.Empty;

    void IDocumentModel.Parse(string documentContent)
    {
        originalContent = documentContent;
        fields = EnvParser.Parse(documentContent);
    }

    IEnumerable<string> IDocumentModel.GetFieldNames()
    {
        return fields.Keys;
    }

    string IDocumentModel.GetFieldRawValue(string fieldName)
    {
        if (fields.TryGetValue(fieldName, out var entry))
        {
            return entry.RawValue;
        }
        throw new KeyNotFoundException($"Field '{fieldName}' not found.");

    }

    bool IDocumentModel.TryGetValueAsString(string fieldName, [NotNullWhen(true)] out string? value)
    {
        var rawValue = (this as IDocumentModel).GetFieldRawValue(fieldName);
        if (rawValue != null)
        {
            value = rawValue;
            return true;
        }
        value = null;
        return false;
    }

    void IDocumentModel.SetFieldRawValue(string fieldName, string rawValue)
    {
        // setting as raw value here as in .env files, the value is by default a string, and we don't have to do any conversion.
        replacements[fieldName] = rawValue;
    }

    void IDocumentModel.SetFieldValue<T>(string fieldName, [MaybeNull] T value)
    {
        (this as IDocumentModel).SetFieldRawValue(fieldName, value?.ToString() ?? string.Empty);
    }

    string IDocumentModel.Serialize()
    {
        var sb = new StringBuilder(originalContent);
        var kvList = fields.ToList().OrderByDescending(kv => kv.Value.ValueStartIdx);
        foreach (var kvp in kvList)
        {
            if (!replacements.TryGetValue(kvp.Key, out var newValue))
            {
                continue;
            }

            if (fields.TryGetValue(kvp.Key, out var position))
            {
                sb.Remove(position.ValueStartIdx, position.ValueLength);
                sb.Insert(position.ValueStartIdx, newValue);
            }
        }
        return sb.ToString();
    }

}
