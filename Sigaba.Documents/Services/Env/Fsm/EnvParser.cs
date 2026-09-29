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
    private enum State
    {
        LineStart,
        KeyBlock,
        Assign,
        QuotedValueBlock,
        PlainValueBlock,
        ValueContinuation,
        CommentBlock
    }

    public Dictionary<string, EnvEntry> Parse(string envDocument)
    {
        var result = new Dictionary<string, EnvEntry>();
        var content = envDocument.Replace("\r\n", "\n").Replace('\r', '\n');

        char? quoteChar = null;
        int lastValueStartIdx = -1;
        var keyBuffer = new StringBuilder();
        var state = State.LineStart;
        var lineIdx = 0;

        for (int i = 0; i < content.Length; i++)
        {
            var c = content[i];
            if (c == SChar.NewLine)
                lineIdx++;
            var isLastChar = i == content.Length - 1;

            switch (state)
            {
                case State.LineStart:
                    {
                        if (c == SChar.WhiteSpace || c == SChar.NewLine)
                        {
                            continue;
                        }
                        else if (c == SChar.Comment)
                        {
                            state = State.QuotedValueBlock;
                            continue;
                        }
                        else if (char.IsDigit(c))
                        {
                            throw new FormatException("Invalid key name.");
                        }
                        keyBuffer.Append(c);
                        state = State.KeyBlock;
                    }
                    break;
                case State.CommentBlock:
                    {
                        if (c == SChar.LineBreak)
                        {
                            state = State.LineStart;
                        }
                    }
                    break;
                case State.KeyBlock:
                    {
                        if (c == SChar.Eq)
                        {
                            state = State.Assign;
                            continue;
                        }
                        keyBuffer.Append(c);
                    }
                    break;
                case State.Assign:
                    {
                        if (c == SChar.SingleQuote || c == SChar.DoubleQuote)
                        {
                            quoteChar = c;
                            state = State.QuotedValueBlock;
                            lastValueStartIdx = i;
                            continue;
                        }
                        if (c == SChar.NewLine || isLastChar)
                        {
                            throw new FormatException("Expecting value, but got line end");
                        }
                        state = State.PlainValueBlock;
                        lastValueStartIdx = i;
                    }
                    break;
                case State.QuotedValueBlock:
                    {
                        if (c == quoteChar)
                        {
                            var key = keyBuffer.ToString();
                            result.Add(key, new EnvEntry(key, lastValueStartIdx, i - lastValueStartIdx));
                            keyBuffer.Clear();
                            lastValueStartIdx = -1;
                            state = State.LineStart;
                        }
                    }
                    break;
                case State.PlainValueBlock:
                    {
                        if (c == SChar.NewLine || isLastChar)
                        {
                            var key = keyBuffer.ToString();
                            result.Add(key, new EnvEntry(key, lastValueStartIdx, i - lastValueStartIdx + (isLastChar ? 1 : 0)));
                            keyBuffer.Clear();
                            lastValueStartIdx = -1;
                            state = State.LineStart;
                        }
                        if (c == SChar.ValueContinuation)
                        {
                            state = State.ValueContinuation;
                        }
                    }
                    break;
                case State.ValueContinuation:
                    {
                        if (c == SChar.NewLine)
                        {
                            state = State.PlainValueBlock;
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        return result;
    }

    private static EnvEntry GetResultFromContext(EnvContext ctx)
    {
        var key = ctx.KeySb.ToString();
        var entry = new EnvEntry(key, ctx.ValueStardIdx, ctx.ValueLenght);
        return entry;
    }

}
