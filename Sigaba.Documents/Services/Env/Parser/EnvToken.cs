using Sigaba.Documents.Services.Env.Parser;

namespace Sigaba.Documents.Services.Env.Parser;

internal record EnvToken(TokenType Type, string Content, int StartIndex, int EndIndex);
