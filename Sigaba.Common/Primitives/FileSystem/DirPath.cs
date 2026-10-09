namespace Sigaba.Primitives.FileSystem;

public record DirPath
{
    public string Value { get; init; }

    public DirPath(params string[] segments)
    {
        Value = Path.Combine(segments);
    }

    public DirPath? Parent()
    {
        var parentDir = Path.GetDirectoryName(Value);
        return parentDir == null ? null : new DirPath(parentDir);
    }

    public static implicit operator DirPath(string value) => new(value);
    public static implicit operator string(DirPath dirPath) => dirPath.Value;

    public override string ToString() => Value;
}
