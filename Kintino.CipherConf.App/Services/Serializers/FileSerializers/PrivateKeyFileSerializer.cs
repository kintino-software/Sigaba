using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.Serializers.FileSerializers;

internal class PrivateKeyFileSerializer(IFileSystem fs, FileContextHelper fileContextHelper) : IPrivateKeyFileSerializer
{
    public const string PrivateKeyFileName = "private.key";

    public async Task<PrivateKey?> LoadPrivateKeyAsync()
    {
        var privateKeyFilePath = fs.Path.Combine(fileContextHelper.SettingsFolderPath, PrivateKeyFileName);
        if (!fs.File.Exists(privateKeyFilePath))
        {
            return null;
        }
        var privateKeyContent = await fs.File.ReadAllTextAsync(privateKeyFilePath);
        var privateKey = PrivateKey.FromBase64(privateKeyContent);
        return privateKey;
    }
    public async Task SavePrivateKeyAsync(PrivateKey privateKey)
    {
        var privateKeyFilePath = fs.Path.Combine(fileContextHelper.SettingsFolderPath, PrivateKeyFileName);
        var privateKeyContent = privateKey.ToBase64();
        await fs.File.WriteAllTextAsync(privateKeyFilePath, privateKeyContent);
    }

}
