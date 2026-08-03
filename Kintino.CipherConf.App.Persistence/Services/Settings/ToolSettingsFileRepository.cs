using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.Settings;

internal class ToolSettingsFileRepository(IFileSystem fs) : IToolSettingsRepository
{
    async Task IToolSettingsRepository.SaveDefaultAsync(string filePath)
    {
        var v1 = ToolSettingsV1.CreateDefault();
        await fs.File.WriteAllTextAsync(filePath, v1.Serialize());
    }

    async Task<IToolSettings?> IToolSettingsRepository.LoadAsync(string filePath)
    {
        if (!fs.File.Exists(filePath)) return null;
        var content = await fs.File.ReadAllTextAsync(filePath);

        var version = JsonHelper.ReadVersionFromJson(content);
        return version switch
        {
            1 => ToolSettingsV1.Deserialize(content),
            _ => throw new NotSupportedException($"ToolSettings version {version} is not supported.")
        };
    }
}
