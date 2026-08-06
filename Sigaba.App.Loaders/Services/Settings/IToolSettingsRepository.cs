namespace Sigaba.App.Services.Settings;

internal interface IToolSettingsRepository
{
    Task<bool> ExistsAsync();
    Task<IToolSettings> LoadAsync();
    Task SaveDefaultAsync();
}
