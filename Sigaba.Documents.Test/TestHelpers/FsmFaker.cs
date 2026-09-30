using Sigaba.Documents.Services.Env.Fsm;

namespace Sigaba.Documents.TestHelpers;

internal static class FsmFaker
{
    public static FsmContext CrateFsmContext(string content = "", EnvToken currToken = null, IEnumerable<EnvToken> tokens = null)
    {
        var ctx = new FsmContext(content)
        {
            CurrToken = currToken,
        };
        if (tokens != null)
            ctx.Tokens.AddRange(tokens);
        return ctx;
    }

}
