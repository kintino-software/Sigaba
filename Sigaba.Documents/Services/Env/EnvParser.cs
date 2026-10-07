using Sigaba.Documents.Services.Env;

using Sigaba.Documents.Services.Env.Fsm;
namespace Sigaba.Documents.Services.Env;

public record EnvEntry(string Key, int ValueStartIdx, int ValueLength);

/// <summary>
/// Based on this spec: https://github.com/env-lang/env/blob/main/env.md
/// </summary>
internal class EnvParser(IFsm fsm)
{
    public IReadOnlyDictionary<string, EnvEntry> Parse(string envDocument)
    {
        var content = envDocument.Replace("\r\n", "\n").Replace('\r', '\n');
        var result = fsm.Process(content);
        return result.ToDictionary(tk => tk.Key, tk => new EnvEntry(tk.Key, tk.RawValueStartIndex, tk.RawValueEndIndex));
    }
}
