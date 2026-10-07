namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

internal class CommentFsm(FsmContext ctx) : ISegmentFsm
{
    private readonly Cursor cursor = ctx.Cursor;

    public ISegmentFsm? Handle()
    {
        char? c;
        while ((c = cursor.Next()) != null)
        {
            if (c == SChar.NewLine)
            {
                return new LineStartFsm(ctx);
            }
        }
        return null;
    }
}