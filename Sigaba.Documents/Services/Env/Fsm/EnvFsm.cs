using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm;

public sealed class EnvFsm
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

    private readonly Dictionary<string, EnvEntry> entries = [];
    public IReadOnlyDictionary<string, EnvEntry> Entries => entries;

    private readonly StringBuilder keyBuffer = new(256);
    private int lastValueStartIdx = -1;
    private State curState = State.LineStart;
    private char? quoteChar = null;
    private bool isLastChar = false;
    private int curIndex = -1;

    public void HandleChar(char c, int curIndex, bool isLastChar)
    {
        this.isLastChar = isLastChar;
        this.curIndex = curIndex;
        curState = curState switch
        {
            State.LineStart => HandleLineStart(c),
            State.CommentBlock => HandleComment(c),
            State.KeyBlock => HandleKeyBlock(c),
            State.Assign => HandleAssign(c),
            State.PlainValueBlock => HandlePlainValue(c),
            State.ValueContinuation => HandleValueContinuation(c),
            State.QuotedValueBlock => HandleQuotedBlock(c),
            _ => throw new NotImplementedException($"Handling of state ${curState} not implemented.")
        };
    }

    private State HandleLineStart(char c)
    {
        if (c == SChar.WhiteSpace || c == SChar.NewLine || c == SChar.Comment)
        {
            return State.LineStart;
        }
        else if (char.IsDigit(c))
        {
            throw new FormatException("Invalid key name.");
        }
        keyBuffer.Append(c);
        return State.KeyBlock;
    }

    private State HandleComment(char c)
    {
        if (c == SChar.LineBreak)
        {
            return State.LineStart;
        }
        return curState;
    }

    private State HandleKeyBlock(char c)
    {
        if (c == SChar.Eq)
        {
            return State.Assign;
        }
        keyBuffer.Append(c);
        return curState;
    }

    private State HandleAssign(char c)
    {
        if (c == SChar.SingleQuote || c == SChar.DoubleQuote)
        {
            quoteChar = c;
            lastValueStartIdx = curIndex;
            return State.QuotedValueBlock;
        }
        if (c == SChar.NewLine || isLastChar)
        {
            throw new FormatException("Expecting value, but got line end");
        }
        lastValueStartIdx = curIndex;
        return State.PlainValueBlock;
    }

    private State HandlePlainValue(char c)
    {
        if (c == SChar.NewLine || isLastChar)
        {
            var key = keyBuffer.ToString();
            entries.Add(key, new EnvEntry(key, lastValueStartIdx, curIndex - lastValueStartIdx + (isLastChar ? 1 : 0)));
            keyBuffer.Clear();
            lastValueStartIdx = -1;
            return State.LineStart;
        }
        if (c == SChar.ValueContinuation)
        {
            return State.ValueContinuation;
        }
        return curState;
    }

    private State HandleValueContinuation(char c)
    {
        if (c == SChar.NewLine)
        {
            return State.PlainValueBlock;
        }
        return curState;
    }

    private State HandleQuotedBlock(char c)
    {
        if (c == quoteChar)
        {
            var key = keyBuffer.ToString();
            entries.Add(key, new EnvEntry(key, lastValueStartIdx, curIndex - lastValueStartIdx));
            keyBuffer.Clear();
            lastValueStartIdx = -1;
            return State.LineStart;
        }
        return curState;
    }


}
