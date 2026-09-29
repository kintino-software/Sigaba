using Sigaba.Documents.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Sigaba.Documents.Services.Env;

internal class EnvDocumentModel : IDocumentModel
{
    private record ValuePosition(int StartIndex, int Length);

    private string originalContent = string.Empty;
    private readonly Dictionary<string, ValuePosition> fields = [];
    private readonly Dictionary<string, string> replacements = [];

    private void ReadFromBytes(byte[] bytes)
    {
        using var memory = new MemoryStream(bytes);
        using var reader = new StreamReader(memory, Encoding.UTF8);

        var c = '\0';
        var idx = -1;
        bool isKey = true;
        var valueContinuesInNextLine = false;

        var keyBuffer = new List<char>(256);
        var lastKey = string.Empty;
        var lastStartIdx = 0;
        var lastLength = 0;

        void ResetState()
        {
            isKey = true;
            valueContinuesInNextLine = false;
            keyBuffer.Clear();
            lastKey = string.Empty;
            lastStartIdx = 0;
            lastLength = 0;
        }

        int read;
        while ((read = reader.Read()) != -1)
        {
            c = (char)read;
            idx++;
            if (c == '=')
            {
                isKey = false;
                lastStartIdx = idx + 1;
                lastKey = new string(keyBuffer.ToArray()).Trim();
                keyBuffer.Clear();

            }
            if (isKey)
            {
                keyBuffer.Add(c);
                continue;
            }
            if (c == '\\')
            {
                valueContinuesInNextLine = true;
                continue;
            }
            if (c == '\n')
            {
                if (isKey)
                {
                    throw new InvalidOperationException("Unexpected newline while parsing key.");
                }
                else
                {
                    if (!valueContinuesInNextLine)
                    {
                        lastLength = idx - lastStartIdx;
                        fields[lastKey] = new ValuePosition(lastStartIdx, lastLength);
                        ResetState();
                        continue;
                    }
                    else
                    {
                        valueContinuesInNextLine = false;
                    }
                }
            }
        }

        if (!isKey)
        {
            lastLength = idx - lastStartIdx + 1; // +1 to include the last character
            fields[lastKey] = new ValuePosition(lastStartIdx, lastLength);
        }
    }

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
