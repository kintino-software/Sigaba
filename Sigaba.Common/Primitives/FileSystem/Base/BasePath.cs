using System.IO.Abstractions;

namespace Sigaba.Primitives.FileSystem.Base;

public abstract class BasePath : IEquatable<BasePath>
{
    public IFileSystem Fs { get; }
    public string Path { get; }
    public bool IsAbsolute => Fs.Path.IsPathFullyQualified(Path);

    protected BasePath(IFileSystem fs, params string[] parts)
    {
        Fs = fs;
        Path = Fs.Path.Combine(parts);
    }

    // equality

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
