using Kintino.CipherConf.App.Services.Settings;
using Kintino.CipherConf.App.Services;

namespace Kintino.CipherConf.App.Services.Settings;

internal interface IToolSettingsRepository
{
    Task<IToolSettings?> LoadAsync(string filePath);
    Task SaveDefaultAsync(string filePath);
}
