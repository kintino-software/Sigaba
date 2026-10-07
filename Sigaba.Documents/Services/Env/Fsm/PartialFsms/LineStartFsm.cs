namespace Sigaba.Documents.Services.Env.Fsm.PartialFsms;

internal class LineStartFsm(FsmContext ctx) : ISegmentFsm
{
    private readonly Cursor cursor = ctx.Cursor;

    public ISegmentFsm? Handle()
    {
        char? c;
        while ((c = cursor.Next()) != null)
        {
            if (c == SChar.Comment)
            {
                return new CommentFsm(ctx);
            }
            if (c == SChar.Space)
            {
                continue;
            }
            if (c == SChar.NewLine)
            {
                return new LineStartFsm(ctx);
            }
            if (KeyFsm.IsValidFirstChar(c.Value))
            {
                return new KeyFsm(ctx);
            }
            throw new FormatException(
                $"Unexpected character '{c}' at line {cursor.LineIndex + 1}, column {cursor.ColumnIndex + 1}.");

        }
        return null;
    }
}
