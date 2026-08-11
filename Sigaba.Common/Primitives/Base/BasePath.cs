using System.IO.Abstractions;

namespace Sigaba.Primitives.Base;

public abstract class BasePath(IFileSystem fs, params string[] parts) : IEquatable<BasePath>
{
    public IFileSystem Fs { get; } = fs;
    public string Path { get; } = SanitizeParts(parts);

    private static string SanitizeParts(string[] parts)
    {
        // as we want to accept both forward and backward slashes in the entire app boundary,
        // we need to split the parts by both types of slashes and then combine them using Path.Combine
        // to ensure the correct path separator is used for the current platform.
        var sanitizedParts = parts.SelectMany(part => part.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries));
        var combined = System.IO.Path.Combine([.. sanitizedParts]);
        if (string.IsNullOrWhiteSpace(combined))
            throw new ArgumentException("Path cannot be null or whitespace.", nameof(parts));
        return combined;
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
