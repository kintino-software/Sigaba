using Kintino.CipherConf.IO.Implementations;
using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;
using System.Text.Json;

namespace Kintino.CipherConf.IO.Services;

internal class ContextSerializer(IFileSystem fs) : IContextSerializer
{
    private record FileFilterWrapper(string? Include, string? Exclude);
    private record SettingsWrapper(string Key, string? FieldRegex, FileFilterWrapper FileRegex);

    public Task SerializeToFileSystem(Context context, string settingsFilePath, string privateKeyFilePath, string publicKeyFilePath)
    {
        var privateKeyStr = context.PrivateKey.Bytes.ToBase64String();
        var publicKeyStr = context.PublicKey.Bytes.ToBase64String();
        var settingsFileContent = JsonSerializer.Serialize(new SettingsWrapper(
            Key: context.Key.Bytes.ToBase64String(),
            FieldRegex: context.FieldFilterImpl.IncludePattern,
            FileRegex: new FileFilterWrapper(
                context.FileFilterImpl.IncludePattern,
                context.FileFilterImpl.ExcludePattern)), JsonConfig.SerializerOptions);

        return Task.WhenAll(
            fs.File.WriteAllTextAsync(settingsFilePath, settingsFileContent),
            fs.File.WriteAllTextAsync(privateKeyFilePath, privateKeyStr),
            fs.File.WriteAllTextAsync(publicKeyFilePath, publicKeyStr));
    }

    public async Task<Context> DeserializeFromFileSystem(string settingsFilePath, string privateKeyFilePath, string publicKeyFilePath)
    {
        var privateKeyStr = await fs.File.ReadAllTextAsync(privateKeyFilePath);
        var publicKeyStr = await fs.File.ReadAllTextAsync(publicKeyFilePath);
        var settingsFileContent = await fs.File.ReadAllTextAsync(settingsFilePath);

        var settings = JsonSerializer.Deserialize<SettingsWrapper>(settingsFileContent, JsonConfig.SerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize settings file.");

        return new Context
        {
            Key = new EncryptedKey(new EncryptedData(settings.Key.FromBase64String())),
            PrivateKey = new PrivateKey(new PlainData(privateKeyStr.FromBase64String())),
            PublicKey = new PublicKey(new PlainData(publicKeyStr.FromBase64String())),
            FieldFilterImpl = new FieldFilter(settings.FieldRegex),
            FileFilterImpl = new FileFilter(settings.FileRegex.Include, settings.FileRegex.Exclude)
        };
    }
}
