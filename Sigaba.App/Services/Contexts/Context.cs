using Sigaba.Primitives;

namespace Sigaba.App.Services.Contexts;

public class Context
{
    public required string SigabaRootDir { get; init; }
    public required string SigabaFilePath { get; init; }
    public required PublicKey PublicKey { get; init; }
    public required PrivateKey? PrivateKey { get; init; }
    public required Predicate<string> FieldFilterPredicate { get; init; }
    public required IEnumerable<string> WorkingSetFiles { get; init; }
}