using System.IO.Abstractions;
using System.Text.Json;

namespace Kintino.CipherConf.App.Services.Serializers.FileSerializers;

internal class ToolSettingsFileSerializer(IFileSystem fs) : IToolSettingsFileSerializer
{
    private readonly static JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task SaveAppSettings(ToolSettingsV1 settings)
    {
        var fileContextHelper = new FileContextHelper(fs);
        var settingsFilePath = fs.Path.Combine(fileContextHelper.SettingsFolderPath, FileContextHelper.SettingsFileName);
        var jsonContent = JsonSerializer.Serialize(settings, serializerOptions);
        await fs.File.WriteAllTextAsync(settingsFilePath, jsonContent);
    }

    public async Task<ToolSettingsV1?> LoadSettingsAsync()
    {
        var fileContextHelper = new FileContextHelper(fs);
        var settingsFilePath = fs.Path.Combine(fileContextHelper.SettingsFolderPath, FileContextHelper.SettingsFileName);
        if (!fs.File.Exists(settingsFilePath)) return null;
        var jsonContent = await fs.File.ReadAllTextAsync(settingsFilePath);
        return JsonSerializer.Deserialize<ToolSettingsV1>(jsonContent, serializerOptions);
    }

}
