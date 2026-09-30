namespace Sigaba.Documents.Services.Env.Fsm;

internal interface IFsm
{
    IFsm? Handle();
}
