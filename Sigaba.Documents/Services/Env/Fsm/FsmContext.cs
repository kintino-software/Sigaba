using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm;

internal class EnvToken
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int ValueStartIndex { get; set; } = -1;
    public int ValueEndIndex { get; set; }
}

internal class FsmContext(string content)
{
    public List<EnvToken> Tokens { get; } = [];
    public EnvToken? CurrToken { get; set; } = null;
    public Cursor Cursor { get; } = new(content);
    public StringBuilder KeyBuffer = new(256);
    public StringBuilder ValueBuffer = new(256);
}
