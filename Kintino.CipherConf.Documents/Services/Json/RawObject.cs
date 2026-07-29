using Kintino.CipherConf.Documents.Services.Json.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kintino.CipherConf.Documents.Services.Json;

[JsonConverter(typeof(RawConverter))]
public class RawObject(List<RawField> fields, Dictionary<string, RawObject> children)
{
    public List<RawField> Fields { get; } = fields ?? throw new ArgumentNullException(nameof(fields));
    public Dictionary<string, RawObject> Children { get; } = children ?? throw new ArgumentNullException(nameof(children));

    public IEnumerable<string> GetFieldPaths()
    {
        return GetFieldPathsCore();
    }

    public RawField? GetFieldByPath(string path)
    {
        var parts = path.Split('.');
        RawObject current = this;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (!current.Children.TryGetValue(parts[i], out var child))
            {
                return null;
            }
            current = child;
        }
        var fieldName = parts[^1];
        var field = current.Fields.FirstOrDefault(f => f.Key == fieldName);
        return field;
    }

    public bool TryGetChild<T>(string key, [NotNullWhen(true)] out T? child, JsonSerializerOptions? serializerOptions = null) where T : class
    {
        child = null;
        if (Children.TryGetValue(key, out var metaChild))
        {
            try
            {
                child = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(metaChild), serializerOptions);
                return child != null;
            }
            catch
            {
                // swallow exception as it is a try-get method
            }
        }
        return false;
    }

    public void SetChild<T>(string key, T child, JsonSerializerOptions? serializerOptions = null)
    {
        var metaChild = JsonSerializer.Deserialize<RawObject>(JsonSerializer.Serialize(child, serializerOptions), serializerOptions);
        Children[key] = metaChild ?? throw new Exception($"Could not serialize child object for key '{key}'");
    }

    private IEnumerable<string> GetFieldPathsCore(string parentPath = "")
    {
        foreach (var field in Fields)
        {
            yield return string.IsNullOrEmpty(parentPath) ? field.Key : $"{parentPath}.{field.Key}";
        }
        foreach (var child in Children)
        {
            var childPath = string.IsNullOrEmpty(parentPath) ? child.Key : $"{parentPath}.{child.Key}";
            foreach (var path in child.Value.GetFieldPathsCore(childPath))
            {
                yield return path;
            }
        }
    }

    private RawObject? GetChildByPath(string path)
    {
        var parts = path.Split('.');
        RawObject current = this;
        for (int i = 0; i < parts.Length; i++)
        {
            if (!current.Children.TryGetValue(parts[i], out var child))
            {
                return null;
            }
            current = child;
        }
        return current;
    }
}
