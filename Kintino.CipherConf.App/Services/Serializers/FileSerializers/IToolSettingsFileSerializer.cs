namespace Kintino.CipherConf.App.Services.Serializers.FileSerializers;

internal interface IToolSettingsFileSerializer
{
    Task<ToolSettingsV1?> LoadSettingsAsync();
    Task SaveAppSettings(ToolSettingsV1 settings);
}