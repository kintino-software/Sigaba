namespace Sigaba.App;

public interface IContextLoader
{
    Task CreateContextAsync();
    Task<IContext?> LoadContextAsync();
}