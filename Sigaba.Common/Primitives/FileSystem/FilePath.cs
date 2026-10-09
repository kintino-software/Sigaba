namespace Sigaba.Primitives.FileSystem;

public record FilePath
{
    public string Value { get; init; }

    public FilePath(params string[] segments)
    {
        Value = Path.Combine(segments);
    }

    public DirPath GetContainingDirectory()
    {
        var directoryPath = Path.GetDirectoryName(Value)
            ?? throw new InvalidOperationException($"Cannot get containing directory for file path '{Value}'");
        return new DirPath(directoryPath);
    }

    public static implicit operator FilePath(string value) => new(value);
    public static implicit operator string(FilePath filePath) => filePath.Value;

    public override string ToString() => Value;
}
