using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm;

internal class FsmContext(string content)
{
    public List<EnvToken> Tokens { get; } = [];
    public EnvToken? CurrToken { get; set; } = null;
    public Cursor Cursor { get; } = new(content);
    public StringBuilder KeyBuffer = new(256);
    public StringBuilder PlainValueBuffer = new(256);
    public StringBuilder RawValueBuffer = new(256);
}
