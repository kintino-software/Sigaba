using Sigaba.Documents.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sigaba.Documents.Services.Env;

internal class EnvDocumentModel : IDocumentModel
{
    private record ValuePosition(int StartIndex, int Length);

    void IDocumentModel.Parse(string documentContent)
    {
        originalContent = documentContent;
        var bytes = Encoding.UTF8.GetBytes(documentContent);
        ReadFromBytes(bytes);
    }

    IEnumerable<string> IDocumentModel.GetFieldNames()
    {
        return fields.Keys;
    }

    string IDocumentModel.GetFieldRawValue(string fieldName)
    {
        if (!fields.TryGetValue(fieldName, out var position))
        {
            return string.Empty;
        }
        return originalContent.Substring(position.StartIndex, position.Length);
    }

    bool IDocumentModel.TryGetValue<T>(string fieldName, out T value)
    {
        if (typeof(T) == typeof(string))
        {
            value = (T)(object)(this as IDocumentModel).GetFieldRawValue(fieldName);
            return true;
        }
        else
        {
            throw new NotSupportedException($"Type '{typeof(T).FullName}' is not supported for env files.");
        }
    }

    void IDocumentModel.SetFieldRawValue(string fieldName, string rawValue)
    {
        if (!fields.ContainsKey(fieldName))
        {
            throw new KeyNotFoundException($"Field '{fieldName}' does not exist.");
        }
        replacements[fieldName] = rawValue;
    }

    void IDocumentModel.SetFieldValue<T>(string fieldName, [MaybeNull] T value)
    {
        (this as IDocumentModel).SetFieldRawValue(fieldName, value?.ToString() ?? string.Empty);
    }

    string IDocumentModel.Serialize()
    {
        var sb = new StringBuilder(originalContent);
        var kvList = fields.ToList().OrderByDescending(kv => kv.Value.StartIndex);
        foreach (var kvp in kvList)
        {
            if (!replacements.TryGetValue(kvp.Key, out var newValue))
            {
                continue;
            }

            if (fields.TryGetValue(kvp.Key, out var position))
            {
                sb.Remove(position.StartIndex, position.Length);
                sb.Insert(position.StartIndex, newValue);
            }
        }
        return sb.ToString();
    }

}
