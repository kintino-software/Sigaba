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

        ctx.PlainValueBuffer.Clear();
        ctx.RawValueBuffer.Clear();

        if (cursor.IsLastChar)
        {
            ApplyTokenValue(ctx, rawValueStartIdx, ctx.Cursor.CurrIndex);
            return null;
        }

        SkipWhiteSpaces(ctx);

        if (cursor.CurrChar == SChar.DoubleQuote || cursor.CurrChar == SChar.SingleQuote)
            return HandleQhotedValue();
        else
            return HandlePlainValue();
    }

    private IFsm? HandlePlainValue()
    {
        do
        {
            if (cursor.CurrChar == SChar.ValueContinuation)
            {
                HandleLineContinuation(ctx); // advances the cursor
                continue;
            }

            if (cursor.CurrChar == SChar.Comment)
            {
                HandleComment(ctx); // advances the cursor

            }

            if (cursor.CurrChar == SChar.Eq)
                throw new FormatException($"Unexpected character '{SChar.Eq}' in value. Expected a plain value or a quoted value.");

            if (cursor.CurrChar == SChar.DoubleQuote || cursor.CurrChar == SChar.SingleQuote)
                throw new FormatException($"Unexpected character '{cursor.CurrChar}' in value. Expected a plain value or a quoted value.");

            if (cursor.CurrChar == SChar.NewLine)
            {
                ApplyTokenValue(ctx, rawValueStartIdx, ctx.Cursor.CurrIndex - 1); // -1 because we don't want to include the new line in the value
                return new LineStartFsm(ctx);
            }

            ctx.RawValueBuffer.Append(cursor.CurrChar);
            ctx.PlainValueBuffer.Append(cursor.CurrChar);

            if (cursor.IsLastChar)
            {
                // there's no next char, so we can apply the token value and return null to indicate that we're done
                ApplyTokenValue(ctx, rawValueStartIdx, ctx.Cursor.CurrIndex);
                return null;
            }

        }
        while (cursor.Next() != null);
        return null;
    }

    private IFsm? HandleQhotedValue()
    {
        var quoteChar = cursor.CurrChar;
        var startIdx = ctx.Cursor.CurrIndex;
        do
        {
            ctx.PlainValueBuffer.Append(cursor.CurrChar); // it will append the quote char for raw value, we can sanitize later

            if (cursor.IsLastChar)
                throw new FormatException($"Unexpected end of input while parsing quoted value. Expected closing quote: {quoteChar}");

            if (cursor.CurrChar == quoteChar)
            {
                ApplyTokenValue(ctx, startIdx, ctx.Cursor.CurrIndex);
                return new LineStartFsm(ctx);
            }
        }
        while (cursor.Next() != null);
        return null;

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

    private static void HandleComment(FsmContext ctx)
    {
        var cursor = ctx.Cursor;

        if (cursor.CurrChar != SChar.Comment)
            throw new Exception($"Unexpected character '{cursor.CurrChar}' while handling comment. Expected a comment character.");

        do
        {
            if (cursor.IsLastChar)
                return;
            ctx.RawValueBuffer.Append(cursor.CurrChar);
        }
        while (cursor.Next() != null);
    }

    private static void HandleLineContinuation(FsmContext ctx)
    {
        var cursor = ctx.Cursor;
        if (cursor.CurrChar != SChar.ValueContinuation)
            throw new Exception($"Unexpected character '{cursor.CurrChar}' while handling line continuation. Expected a value continuation character.");
        while (cursor.Next() != null)
        {
            ctx.RawValueBuffer.Append(cursor.CurrChar);

            if (cursor.CurrChar == SChar.NewLine)
                return;

            if (cursor.CurrChar != SChar.WhiteSpace)
                throw new FormatException($"Only white space characters are allowed after a line continuation. Unexpected character: '{cursor.CurrChar}'.");

            if (cursor.IsLastChar)
                throw new FormatException($"Unexpected end of file while handling line continuation. Expected a new line character.");

        }
    }

}
