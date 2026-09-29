using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm;

public static class SChar
{
    public readonly static char WhiteSpace = ' ';
    public readonly static char ValueContinuation = '\\';
    public readonly static char Comment = '#';
    public readonly static char NewLine = '\n';
    public readonly static char DoubleQuote = '#';
    public readonly static char SingleQuote = '\'';
    public readonly static char Eq = '=';
    public readonly static char LineBreak = '\n';
}

public record EnvEntry(string Key, int ValueStartIdx, int ValueLength);

public class EnvContext
{
    public readonly StringBuilder KeySb = new();
    public int ValueStardIdx = -1;
    public int ValueLenght = 0;

    public void Reset()
    {
        KeySb.Clear();
        ValueStardIdx = -1;
        ValueLenght = 0;
    }
}

public interface IEnvState
{
    public bool IsTerminal { get; }
    IEnvState ProcessChar(char c, int charIdx, EnvContext ctx);
}

public sealed class PlainValueEndState : IEnvState
{
    public bool IsTerminal { get; } = true;

    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        return new StartLineState();
    }
}

public sealed class CommentState : IEnvState
{
    public bool IsTerminal => false;
    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        if (c == SChar.LineBreak)
        {
            return new StartLineState();
        }
        return this;
    }
}

public sealed class PlainValueState : IEnvState
{
    public bool IsTerminal { get; } = false;

    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        ctx.ValueLenght++;
        if (c == SChar.LineBreak)
        {
            return new PlainValueEndState();
        }
        return this;
    }
}


public sealed class QuotedValueEndState : IEnvState
{
    public bool IsTerminal => true;

    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        return new StartLineState();
    }
}

public sealed class QuotedValueState(char quoteChar) : IEnvState
{
    public bool IsTerminal { get; } = false;
    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        if (c == SChar.NewLine)
        {
            throw new FormatException("Expecting value, but found end of line.");
        }
        if (c == quoteChar)
        {
            return new QuotedValueEndState();
        }
        else
        {

        }
        return this;
    }
}

public sealed class AssignState : IEnvState
{
    public bool IsTerminal { get; } = false;
    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        if (c == SChar.DoubleQuote || c == SChar.SingleQuote)
        {
            ctx.ValueStardIdx = charIdx;
            return new QuotedValueState(c);
        }
        ctx.ValueStardIdx = charIdx;
        ctx.ValueLenght++;
        return new PlainValueState();
    }
}

public sealed class KeyState : IEnvState
{
    public bool IsTerminal { get; } = false;
    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        if (c == SChar.Eq)
        {
            return new AssignState();
        }
        ctx.KeySb.Append(c);
        return this;
    }
}

public sealed class StartLineState : IEnvState
{
    public bool IsTerminal { get; } = false;
    public IEnvState ProcessChar(char c, int charIdx, EnvContext ctx)
    {
        if (c == SChar.WhiteSpace)
        {
            return this;
        }
        if (c == SChar.Comment)
        {
            return new CommentState();
        }
        if (char.IsNumber(c) || char.IsDigit(c) || c == SChar.Eq)
        {
            throw new FormatException("Invalid key name");
        }

        ctx.KeySb.Append(c);
        return new KeyState();
    }
}

public class EnvParser
{
    public Dictionary<string, EnvEntry> Parse(string envDocument)
    {
        var result = new Dictionary<string, EnvEntry>();
        var content = envDocument.Replace("\r\n", "\n").Replace('\r', '\n');

        var ctx = new EnvContext();
        IEnvState state = new StartLineState();

        for (int i = 0; i < content.Length; i++)
        {
            var c = content[i];

            if (state.IsTerminal)
            {
                var entry = GetResultFromContext(ctx);
                result.Add(entry.Key, entry);
                ctx.Reset();
            }
            try
            {
                state = state.ProcessChar(c, i, ctx);
            }
            catch (Exception ex) when (ex is FormatException)
            {
                throw new EnvParseException(1, 1, ex.Message);
            }
        }
        var e = GetResultFromContext(ctx);
        result.Add(e.Key, e);

        return result;
    }

    private static EnvEntry GetResultFromContext(EnvContext ctx)
    {
        var key = ctx.KeySb.ToString();
        var entry = new EnvEntry(key, ctx.ValueStardIdx, ctx.ValueLenght);
        return entry;
    }

}
