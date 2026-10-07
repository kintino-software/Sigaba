namespace Sigaba.Documents.Services.Env.Fsm;

public static class SChar
{
    public readonly static char Space = ' '; // todo: it could be a tab: create an extension to check both
    public readonly static char Tab = '\t';
    public readonly static char ValueContinuation = '\\';
    public readonly static char Comment = '#';
    public readonly static char NewLine = '\n';
    public readonly static char DoubleQuote = '#';
    public readonly static char SingleQuote = '\'';
    public readonly static char Eq = '=';
    public readonly static char LineBreak = '\n';
}
