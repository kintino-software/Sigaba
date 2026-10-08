namespace Sigaba.Documents.Services.Env.Interpreter;

internal record EnvToken(TokenType Type, string Content, int StartIndex, int EndIndex);
