namespace Sigaba.Documents.Services.Env.Interpreter;

public sealed class EnvParseException(int line, int column, string message) : Exception(message)
{
    public int Line { get; } = line;
    public int Column { get; } = column;
}
