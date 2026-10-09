using Sigaba.Primitives.FileSystem;

namespace Sigaba.App;

public record CipherResult(IEnumerable<FilePath> PathsOfFilesAffected);
