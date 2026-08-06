namespace Sigaba.App.Services.Settings;

internal interface IToolSettingsManager
{
    Task<bool> ExistsAsync();
    Task<IToolSettings> LoadAsync();
    Task SaveDefaultAsync();
}
