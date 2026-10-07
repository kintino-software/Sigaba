using System.Text;

namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

/// <summary>
/// Spec: <see href="https://github.com/env-lang/env/blob/main/env.md#values"/>
/// </summary>
internal class ValueFsm : ISegmentFsm
{
    private readonly FsmContext ctx;
    private readonly Cursor cursor;
    private readonly StringBuilder buffer;

    private int valueStartIdx = -1;
    private bool hasContinuation = false;
    private char? quoteChar = null;
    private int quoteCharCount = 0;

    public ValueFsm(FsmContext ctx)
    {
        this.ctx = ctx;
        this.cursor = ctx.Cursor;
        this.buffer = ctx.ValueBuffer;

    }

    public ISegmentFsm? Handle()
    {
        SkipInitialSpaces(ctx);

        char? c;
        while ((c = cursor.Next()) != null)
        {
            if (valueStartIdx < 0)
                valueStartIdx = cursor.CurrIndex;

            if (c == SChar.ValueContinuation)
            {
                if (quoteChar != null)
                {
                    buffer.Append(c);
                    return this;
                }
                hasContinuation = true;
                buffer.Append(SChar.NewLine);
                return this;
            }
            if (c == SChar.DoubleQuote || c == SChar.SingleQuote)
            {
                quoteChar ??= c;
                if (c == quoteChar)
                    quoteCharCount++;
                return this;
            }
            if (c == SChar.NewLine)
            {
                if (hasContinuation)
                {
                    hasContinuation = false;
                    continue;
                }
                else
                {
                    break;
                }
            }
            buffer.Append(c);
        }

        if (ctx.CurrToken == null)
            throw new InvalidOperationException("Current token is null. Can't apply value.");

        ctx.CurrToken.RawValueStartIndex = valueStartIdx;
        ctx.CurrToken.RawValueEndIndex = cursor.CurrIndex;
        ctx.CurrToken.Value = buffer.ToString();
        buffer.Clear();
        ctx.Tokens.Add(ctx.CurrToken);
        ctx.CurrToken = null;
        return new LineStartFsm(ctx);
    }

    public static void SkipInitialSpaces(FsmContext ctx)
    {
        var cursor = ctx.Cursor;
        char? c = cursor.Peek();

        do
        {
            if (c == null || c == SChar.Space || c == SChar.Tab)
                continue;
            else
            {
                return;
            }
        }
        while ((c = cursor.Next()) != null);
    }

    public void ApplyValueToToken(int endIndex)
    {
        if (ctx.CurrToken == null)
            throw new InvalidOperationException("Current token is null. Can't apply value.");

        ctx.CurrToken.RawValueStartIndex = valueStartIdx;
        ctx.CurrToken.RawValueEndIndex = endIndex;
        ctx.Tokens.Add(ctx.CurrToken);
        ctx.CurrToken = null;
    }

}
