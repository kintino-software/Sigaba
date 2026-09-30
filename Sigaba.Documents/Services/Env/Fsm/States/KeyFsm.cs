namespace Sigaba.Documents.Services.Env.Fsm.States;

/// <summary>
/// Spec: <see href="https://github.com/env-lang/env/blob/main/env.md#keys"/>
/// </summary>
internal class KeyFsm(FsmContext ctx) : IFsm
{
    private readonly Cursor cursor = ctx.Cursor;

    public static bool IsValidFirstChar(char c) => char.IsLetter(c) || c == '_';

    public IFsm? Handle()
    {
        ctx.CurrToken = null;
        ctx.KeyBuffer.Clear();

        AssertIsValidFirstChar(cursor.CurrChar);

        ctx.KeyBuffer.Append(cursor.CurrChar); // we are already in the fist char of the key, so we append it before moving forward

        while (cursor.Next() != SChar.Eq)
        {
            AssertIsSingleLine(cursor.CurrChar); // cant contain line breaks
            AssertIsValidKeyChar(cursor.CurrChar); // has valid names
            AssertIsNotLastChar(cursor.IsLastChar); // cant end file without a key assignment
            ctx.KeyBuffer.Append(cursor.CurrChar);
        }

        ctx.CurrToken = new EnvToken
        {
            Key = ctx.KeyBuffer.ToString(),
        };
        AssertWontGenerateEmptyKey(ctx.CurrToken.Key); // key should not be empty
        ctx.KeyBuffer.Clear();
        cursor.Next(); // past the "equals" char to value fsm can handle the value start char
        return new ValueFsm(ctx);
    }


    private static void AssertIsValidFirstChar(char c)
    {
        if (IsValidFirstChar(c))
        {
            return;
        }
        throw new FormatException($"Invalid key name. Keys must start with a letter or an underscore (_), but found '{c}'.");
    }

    private static void AssertIsValidKeyChar(char c)
    {
        if (char.IsLetterOrDigit(c) || c == '_')
        {
            return;
        }
        throw new FormatException($"Invalid key name. Keys should contain only letters, numbers or underscore (_), but found '{c}'.");
    }

    private static void AssertIsSingleLine(char c)
    {
        if (c == SChar.NewLine)
        {
            throw new FormatException("Invalid key name. Keys should be single line, but found a line break.");
        }
    }

    public static void AssertWontGenerateEmptyKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new FormatException("Invalid key name. Keys should not be empty.");
        }
    }

    public static void AssertIsNotLastChar(bool isLastChar)
    {
        if (isLastChar)
        {
            throw new FormatException("Invalid key name. Keys should not be empty.");
        }
    }
}
