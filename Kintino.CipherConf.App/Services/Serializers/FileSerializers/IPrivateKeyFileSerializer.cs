using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.Serializers.FileSerializers;

internal interface IPrivateKeyFileSerializer
{
    Task<PrivateKey?> LoadPrivateKeyAsync();
    Task SavePrivateKeyAsync(PrivateKey privateKey);
}