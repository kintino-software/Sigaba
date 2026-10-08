using Sigaba.Documents.Services.Env.Interpreter;

namespace Sigaba.Documents.Services.Env;

internal record EnvToken(TokenType Type, string Content, int StartIndex, int EndIndex);
