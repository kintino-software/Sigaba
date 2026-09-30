namespace Sigaba.Documents.Services.Env.Fsm.States;

internal class CommentFsm(FsmContext ctx) : IFsm
{
    private readonly Cursor cursor = ctx.Cursor;

    public IFsm? Handle()
    {
        while (cursor.CurrChar != SChar.NewLine)
        {
            if (cursor.IsLastChar)
            {
                return null;
            }
            cursor.Next();
        }
        return new LineStartFsm(ctx);
    }
}