namespace Sigaba.Documents.Services.Env.Fsm;

internal class EnvToken
{
    public string Key { get; set; } = string.Empty;
    public string ParsedValue { get; set; } = string.Empty;
    public string RawValue { get; set; } = string.Empty;
    public int RawValueStartIndex { get; set; } = -1;
    public int RawValueEndIndex { get; set; }
}
