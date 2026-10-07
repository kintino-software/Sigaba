using Sigaba.Documents.Services.Env.Fsm.PartialFsms;

namespace Sigaba.Documents.Services.Env.Fsm;

internal interface IFsm
{
    IReadOnlyList<EnvToken> Process(string content);
}

internal class Fsm : IFsm
{
    IReadOnlyList<EnvToken> IFsm.Process(string content)
    {
        if (string.IsNullOrEmpty(content))
            return [];

        var ctx = new FsmContext(content);
        ISegmentFsm? segment = new LineStartFsm(ctx);
        while (segment != null)
            segment = segment.Handle();
        return ctx.Tokens;
    }
}
