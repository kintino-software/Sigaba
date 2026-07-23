using Kintino.CipherConf.IO.Implementations;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;
using System.Text.Json;

namespace Kintino.CipherConf.IO.Services;

internal class ContextSerializer(IFileSystem fs) : IContextSerializer
{
    private record FileFilterWrapper(string? Include, string? Exclude);
    private record SettingsWrapper(string? FieldRegex, FileFilterWrapper FileRegex);

    public async Task SerializeToFileSystem(Context context, string settingsFilePath, string privateKeyFilePath, string publicKeyFilePath)
    {
        var privateKeyStr = context.PrivateKey?.Bytes.ToBase64String();
        var publicKeyStr = context.PublicKey?.Bytes.ToBase64String();
        var settingsFileContent = JsonSerializer.Serialize(new SettingsWrapper(
            FieldRegex: context.FieldFilterImpl.IncludePattern,
            FileRegex: new FileFilterWrapper(
                context.FileFilterImpl.IncludePattern,
                context.FileFilterImpl.ExcludePattern)), JsonConfig.SerializerOptions);

        await fs.File.WriteAllTextAsync(settingsFilePath, settingsFileContent);
        if (privateKeyStr != null)
            await fs.File.WriteAllTextAsync(privateKeyFilePath, privateKeyStr);
        if (publicKeyStr != null)
            await fs.File.WriteAllTextAsync(publicKeyFilePath, publicKeyStr);
    }

    public async Task<Context> DeserializeFromFileSystem(string settingsFilePath, string privateKeyFilePath, string publicKeyFilePath)
    {
        string? privateKeyStr = null;
        string? publicKeyStr = null;
        if (fs.File.Exists(privateKeyFilePath))
            privateKeyStr = await fs.File.ReadAllTextAsync(privateKeyFilePath);
        if (fs.File.Exists(publicKeyFilePath))
            publicKeyStr = await fs.File.ReadAllTextAsync(publicKeyFilePath);
        var settingsFileContent = await fs.File.ReadAllTextAsync(settingsFilePath);

        var settings = JsonSerializer.Deserialize<SettingsWrapper>(settingsFileContent, JsonConfig.SerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize settings file.");

        return new Context
        {
            PrivateKey = privateKeyStr == null ? null : new PrivateKey(new PlainData(privateKeyStr.FromBase64String())),
            PublicKey = publicKeyStr == null ? null : new PublicKey(new PlainData(publicKeyStr.FromBase64String())),
            FieldFilterImpl = new FieldFilter(settings.FieldRegex),
            FileFilterImpl = new FileFilter(settings.FileRegex.Include, settings.FileRegex.Exclude)
        };
    }
}
