using System.IO.Abstractions;
using Kintino.CipherConf.App.Services.Settings;

namespace Kintino.CipherConf.App.Services.Settings;

internal class ToolSettingsFileRepository(IFileSystem fs) : IToolSettingsRepository
{
    async Task IToolSettingsRepository.SaveDefaultAsync(string filePath)
    {
        await fs.File.WriteAllTextAsync(filePath, ToolSettings.SerializeDefault());
    }

    async Task<IToolSettings?> IToolSettingsRepository.LoadAsync(string filePath)
    {
        if (!fs.File.Exists(filePath)) return null;
        var jsonContent = await fs.File.ReadAllTextAsync(filePath);
        return ToolSettings.CreateFromSerialized(
            jsonContent,
            fs.Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException("Directory name could not be determined."),
            fs);
    }
}
