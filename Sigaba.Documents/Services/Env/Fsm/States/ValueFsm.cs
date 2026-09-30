namespace Sigaba.Documents.Services.Env.Fsm.States;

/// <summary>
/// Spec: <see href="https://github.com/env-lang/env/blob/main/env.md#values"/>
/// </summary>
internal class ValueFsm(FsmContext ctx) : IFsm
{
    private readonly Cursor cursor = ctx.Cursor;
    private readonly int rawValueStartIdx = ctx.Cursor.CurrIndex;

    public IFsm? Handle()
    {
        if (cursor.IsLastChar)
        {
            // return early: it's end of file, so value will be always empty
            ApplyTokenValue(ctx, rawValueStartIdx, ctx.Cursor.CurrIndex);
            return null;
        }

        // real value only starts after white spaces, so we can skip them
        SkipWhiteSpaces(ctx);

        if (cursor.CurrChar == SChar.DoubleQuote || cursor.CurrChar == SChar.SingleQuote)
            HandleQuotedValue();
        else
            HandlePlainValue();

        var lastIndexAdjust = cursor.CurrChar == SChar.NewLine ? -1 : 0;
        ApplyTokenValue(ctx, rawValueStartIdx, ctx.Cursor.CurrIndex + lastIndexAdjust);

        if (cursor.CurrChar == SChar.NewLine)
            return new LineStartFsm(ctx);

        return null;
    }

    private void HandlePlainValue()
    {
        do
        {
            if (cursor.CurrChar == SChar.ValueContinuation)
            {
                HandleContinuationValue();
                return;
            }

            if (cursor.CurrChar == SChar.Comment)
            {
                HandleComment(ctx);
                return;
            }

            if (cursor.CurrChar == SChar.Eq)
                throw new FormatException($"Unexpected character '{SChar.Eq}' in value. Expected a plain value or a quoted value.");

            if (cursor.CurrChar == SChar.DoubleQuote || cursor.CurrChar == SChar.SingleQuote)
                throw new FormatException($"Unexpected character '{cursor.CurrChar}' in value. Expected a plain value or a quoted value.");

            if (cursor.CurrChar == SChar.NewLine)
                return;

            ctx.RawValueBuffer.Append(cursor.CurrChar);
            ctx.PlainValueBuffer.Append(cursor.CurrChar);
        }
        while (cursor.Next() != null);
    }

    private static void HandleComment(FsmContext ctx)
    {
        var cursor = ctx.Cursor;

        if (cursor.CurrChar != SChar.Comment)
            throw new Exception("Expecting a existing token");

        if (cursor.CurrChar != SChar.Comment)
            throw new Exception($"Unexpected character '{cursor.CurrChar}' while handling comment. Expected a comment character.");

        do
        {
            if (cursor.CurrChar == SChar.NewLine)
                return;
            ctx.RawValueBuffer.Append(cursor.CurrChar);
        }
        while (cursor.Next() != null);
    }

    private void HandleContinuationValue()
    {
        do
        {

            if (cursor.CurrChar == SChar.ValueContinuation)
            {
                throw new FormatException("Unexpected character '\\' in value. Only white space characters are allowed after a line continuation.");
            }
            if (cursor.CurrChar == SChar.Comment)
            {
                throw new FormatException("Unexpected character '#' in value. Only white space characters are allowed after a line continuation.");
            }
            if (cursor.CurrChar == SChar.NewLine)
            {
                HandlePlainValue();
                return;
            }

            ctx.RawValueBuffer.Append(cursor.CurrChar);
        }
        while (cursor.Next() != null);
    }

    private void HandleQuotedValue()
    {
        var quoteChar = cursor.CurrChar;
        do
        {
            if (cursor.CurrChar == quoteChar)
            {
                return;
            }

            ctx.PlainValueBuffer.Append(cursor.CurrChar);
            ctx.RawValueBuffer.Append(cursor.CurrChar);
        }
        while (cursor.Next() != null);

        throw new FormatException($"Unexpected end of input while parsing quoted value. Expected closing quote: {quoteChar}");
    }

    private static void ApplyTokenValue(FsmContext ctx, int startIndex, int endIndex)
    {
        if (ctx.CurrToken == null)
            throw new Exception("Expecting a existing token");

        ctx.CurrToken.RawValueStartIndex = startIndex;
        ctx.CurrToken.RawValueEndIndex = endIndex;
        ctx.CurrToken.ParsedValue = ctx.PlainValueBuffer.ToString();
        ctx.CurrToken.RawValue = ctx.RawValueBuffer.ToString();
        ctx.Tokens.Add(ctx.CurrToken);

        // cleanup
        ctx.RawValueBuffer.Clear();
        ctx.PlainValueBuffer.Clear();
        ctx.CurrToken = null;
    }

    private static void SkipWhiteSpaces(FsmContext ctx)
    {
        while (ctx.Cursor.CurrChar == SChar.WhiteSpace)
        {
            ctx.RawValueBuffer.Append(ctx.Cursor.CurrChar);
            if (ctx.Cursor.IsLastChar)
                return;
            ctx.Cursor.Next();
        }
    }

}
