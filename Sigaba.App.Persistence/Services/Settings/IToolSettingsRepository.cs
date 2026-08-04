namespace Sigaba.App.Services.Settings;

internal interface IToolSettingsRepository
{
    Task<IToolSettings?> LoadAsync(string filePath);
    Task SaveDefaultAsync(string filePath);
}
