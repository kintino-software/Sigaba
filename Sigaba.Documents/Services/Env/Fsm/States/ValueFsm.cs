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
                SkipWhiteSpaces(ctx);
                if (cursor.CurrChar != SChar.NewLine)
                    throw new FormatException($"Unexpected character '{cursor.CurrChar}' after line continuation. Expected a new line.");
                ctx.RawValueBuffer.Append(cursor.Next()); // append to raw value and skip the new line
                continue;
            }

            if (cursor.CurrChar == SChar.Eq)
                throw new FormatException($"Unexpected character '{SChar.Eq}' in value. Expected a plain value or a quoted value.");

            if (cursor.CurrChar == SChar.NewLine)
            {
                break;
            }

            ctx.RawValueBuffer.Append(cursor.CurrChar);
            ctx.PlainValueBuffer.Append(cursor.CurrChar);
        }
        while (cursor.Next() != null);

        ApplyTokenValue(ctx, rawValueStartIdx, ctx.Cursor.CurrIndex);

        if (cursor.IsLastChar)
            return null;
        return new LineStartFsm(ctx);
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
        while (!ctx.Cursor.IsLastChar && ctx.Cursor.CurrChar == SChar.WhiteSpace)
        {
            ctx.RawValueBuffer.Append(ctx.Cursor.CurrChar);
            ctx.Cursor.Next();
        }
    }

}
