namespace Sigaba.Documents.Services.Env.Interpreter;

public enum TokenType
{
    None = 0,
    Key, Value,
    MultiLineValue,
    LiteralValue,
    EscapedLiteralValue,
    Comment
}
