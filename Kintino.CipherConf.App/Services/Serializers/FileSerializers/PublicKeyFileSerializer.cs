using Kintino.CipherConf.Primitives;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.Serializers.FileSerializers;

internal class PublicKeyFileSerializer(IFileSystem fs, FileContextHelper fileContextHelper) : IPublicKeyFileSerializer
{
    public const string PublicKeyFileName = "public.key";

    public async Task<PublicKey?> LoadPublicKeyAsync()
    {
        var publicKeyFilePath = fs.Path.Combine(fileContextHelper.SettingsFolderPath, PublicKeyFileName);
        if (!fs.File.Exists(publicKeyFilePath))
        {
            return null;
        }
        var publicKeyContent = await fs.File.ReadAllTextAsync(publicKeyFilePath);
        var publicKey = PublicKey.FromBase64(publicKeyContent);
        return publicKey;
    }

    public async Task SavePublicKeyAsync(PublicKey publicKey)
    {
        var publicKeyFilePath = fs.Path.Combine(fileContextHelper.SettingsFolderPath, PublicKeyFileName);
        var publicKeyContent = publicKey.ToBase64();
        await fs.File.WriteAllTextAsync(publicKeyFilePath, publicKeyContent);
    }

}
