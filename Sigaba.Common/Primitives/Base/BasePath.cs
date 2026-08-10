namespace Sigaba.Primitives.Base;

public abstract class BasePath : IEquatable<BasePath>
{
    public string Path { get; }

    public BasePath(params string[] parts)
    {
        var splited = parts.SelectMany(part => part.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries));
        var combined = System.IO.Path.Combine([.. splited]);
        if (string.IsNullOrWhiteSpace(combined))
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(parts));
        Path = combined;
    }

    public bool Equals(BasePath? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Path == other.Path;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BasePath);
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    public override string ToString()
    {
        return Path;
    }

    public static bool operator ==(BasePath? left, BasePath? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(BasePath? left, BasePath? right)
    {
        return !(left == right);
    }
}
