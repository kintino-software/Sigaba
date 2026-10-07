namespace Sigaba.Documents.Services.Env.Fsm;

internal class EnvToken
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int RawValueStartIndex { get; set; } = 0;
    public int RawValueEndIndex { get; set; } = 0;
}
