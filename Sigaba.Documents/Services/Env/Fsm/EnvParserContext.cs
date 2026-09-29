using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm;

public sealed class EnvParserContext
{
    public StringBuilder CurrentLineKey { get; } = new();
    public StringBuilder CurrentLineValue { get; } = new();
    public StringBuilder MultiLineValue { get; } = new();
    public int LineNumber { get; set; } = 1;
    public int LineStartLine { get; set; } = 1;
    public List<EnvEntry> Entries { get; } = [];
    public bool SalWhiteSpaceAfterBacklash { get; set; }
}
