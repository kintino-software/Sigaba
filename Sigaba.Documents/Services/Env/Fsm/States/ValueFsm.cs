namespace Sigaba.Documents.Services.Env.Fsm.States;

internal class ValueFsm(FsmContext ctx) : IFsm
{
    public IFsm Handle()
    {
        if (ctx.CurrToken == null)
            throw new Exception("Expecting a existing token");

        if (ctx.Cursor.CurrChar == SChar.LineBreak || ctx.Cursor.IsLastChar)
        {
            ctx.CurrToken.ValueStartIndex = ctx.Cursor.CurrIndex;
            ctx.CurrToken.ValueEndIndex = ctx.Cursor.CurrIndex;
            ctx.CurrToken.Value = string.Empty;
        }

        throw new NotImplementedException();
    }
}
