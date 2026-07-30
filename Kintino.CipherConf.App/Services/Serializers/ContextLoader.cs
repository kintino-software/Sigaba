using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Services.Serializers.FileSerializers;
using Kintino.CipherConf.Primitives;
using System.Text.RegularExpressions;

namespace Kintino.CipherConf.App.Services.Serializers;

internal class ContextLoader(
    IToolSettingsFileSerializer toolSettingsFileSerializer,
    IPublicKeyFileSerializer publicKeyFileSerializer,
    IPrivateKeyFileSerializer privateKeyFileSerializer,
    FileContextHelper fileContextHelper) : IContextLoader
{
    public async Task<Context?> LoadContextAsync()
    {
        var toolSettings = await toolSettingsFileSerializer.LoadSettingsAsync();
        if (toolSettings == null)
            return null;
        var publicKey = await publicKeyFileSerializer.LoadPublicKeyAsync();
        var privateKey = await privateKeyFileSerializer.LoadPrivateKeyAsync();

        return new Context()
        {
            SettingsVersion = toolSettings.Version,
            AppContextDirectory = fileContextHelper.SettingsFolderPath,
            FieldRegex = new Regex(toolSettings.FieldRegex),
            IncludeFileGlob = toolSettings.IncludeFileGlob,
            ExcludeFileGlob = toolSettings.ExcludeFileGlob,
            PrivateKey = privateKey,
            PublicKey = publicKey
        };
    }

    public async Task CreateContextAsync(PublicKey publicKey, PrivateKey privateKey)
    {
        var defaultSettings = new ToolSettingsV1()
        {
            FieldRegex = "_secret$",
            IncludeFileGlob = [
                @"**/*.secrets.json"
            ],
            ExcludeFileGlob = [
                @"**/bin/**",
                @"**/obj/**",
                @"**/temp/**",
                @"**/test?(s)/**"
            ],
        };
        await toolSettingsFileSerializer.SaveAppSettings(defaultSettings);
        await privateKeyFileSerializer.SavePrivateKeyAsync(privateKey);
        await publicKeyFileSerializer.SavePublicKeyAsync(publicKey);
    }

    public async Task<bool> HasContextAsync()
    {
        return (await toolSettingsFileSerializer.LoadSettingsAsync()) != null;
    }
}
