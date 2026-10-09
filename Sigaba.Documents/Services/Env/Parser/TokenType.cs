namespace Sigaba.Documents.Services.Env.Parser;

public enum TokenType
{
    None = 0,
    Key, Value,
    MultiLineValue,
    LiteralValue,
    EscapedLiteralValue,
    Comment
}
