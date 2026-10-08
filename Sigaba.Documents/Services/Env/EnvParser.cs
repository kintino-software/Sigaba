namespace Sigaba.Documents.Services.Env;

public record EnvEntry(string Key, int ValueStartIdx, int ValueLength);

/// <summary>
/// Based on this spec: https://github.com/env-lang/env/blob/main/env.md
/// </summary>
internal class EnvParser
{
    public IReadOnlyDictionary<string, EnvEntry> Parse(string envDocument)
    {
        throw new NotImplementedException();
    }
}
