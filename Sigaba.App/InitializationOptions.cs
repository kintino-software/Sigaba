using Sigaba.Primitives.FileSystem;

namespace Sigaba.App;

public record InitializationOptions(DirPath SigabaFileOutputDir, string PrivateKeyPassword);
