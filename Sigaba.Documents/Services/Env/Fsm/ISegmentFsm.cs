namespace Sigaba.Documents.Services.Env.Fsm;

internal interface ISegmentFsm
{
    ISegmentFsm? Handle();
}
