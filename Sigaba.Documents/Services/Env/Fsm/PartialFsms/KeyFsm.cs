namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

/// <summary>
/// Spec: <see href="https://github.com/env-lang/env/blob/main/env.md#keys"/>
/// </summary>
internal class KeyFsm : ISegmentFsm
{
    private readonly FsmContext ctx;
    private readonly Cursor cursor;
    private readonly int startingIndex;

    public KeyFsm(FsmContext ctx)
    {
        this.ctx = ctx;
        this.cursor = ctx.Cursor;
        this.startingIndex = cursor.CurrIndex;
        // reset the current token and key buffer when entering the key state
        ctx.CurrToken = null;
        ctx.KeyBuffer.Clear();
    }

    public ISegmentFsm? Handle()
    {
        AssertIsValidFirstChar(cursor.Peek()); // first char must be valid

        char? c;
        while ((c = cursor.Next()) != null)
        {
            if (c == SChar.Eq)
            {
                ctx.CurrToken = new EnvToken
                {
                    Key = ctx.KeyBuffer.ToString(),
                };
                AssertWontGenerateEmptyKey(ctx.CurrToken.Key); // key should not be empty
                ctx.KeyBuffer.Clear();
                return new ValueFsm(ctx);
            }
            AssertIsSingleLine(c.Value); // cant contain line breaks
            AssertIsValidKeyChar(c.Value); // has valid names
            ctx.KeyBuffer.Append(c.Value);
        }

        throw new FormatException("Keys should have a value.");

    }

    public static bool IsValidFirstChar(char? c) => c.HasValue && (char.IsLetter(c.Value) || c.Value == '_');

    private static void AssertIsValidKeyChar(char c)
    {
        if (char.IsLetterOrDigit(c) || c == '_')
        {
            return;
        }
        throw new FormatException($"Invalid key name. Keys should contain only letters, numbers or underscore (_), but found '{c}'.");
    }

    private static void AssertIsValidFirstChar(char? c)
    {
        if (!IsValidFirstChar(c))
        {
            throw new FormatException($"Invalid key name. Keys must start with a letter or an underscore (_), but found '{c}'.");
        }
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
