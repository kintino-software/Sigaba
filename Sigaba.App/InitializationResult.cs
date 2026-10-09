using Sigaba.Primitives.FileSystem;

namespace Sigaba.App;

public record InitializationResult(FilePath SigabaFileLocation, FilePath PrivateKeyLocation);
