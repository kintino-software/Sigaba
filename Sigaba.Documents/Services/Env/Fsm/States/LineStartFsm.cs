namespace Sigaba.Documents.Services.Env.Fsm.States;

internal class LineStartFsm(FsmContext ctx) : IFsm
{
    private readonly Cursor cursor = ctx.Cursor;

    public IFsm? Handle()
    {
        do
        {
            if (cursor.CurrChar == SChar.Comment)
            {
                return new CommentFsm(ctx);
            }
            if (cursor.CurrChar == SChar.NewLine)
            {
                return new LineStartFsm(ctx);
            }
            if (KeyFsm.IsValidFirstChar(cursor.CurrChar))
            {
                return new KeyFsm(ctx);
            }
            if (cursor.CurrChar != SChar.WhiteSpace) // at this point, only whitespace is allowed
            {
                throw new FormatException("Unexpected token.");
            }
        }
        while (cursor.Next() != null);
        return null;
    }
}
