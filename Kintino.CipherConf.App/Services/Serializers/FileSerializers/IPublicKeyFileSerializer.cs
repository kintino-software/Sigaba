using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.Serializers.FileSerializers;

internal interface IPublicKeyFileSerializer
{
    Task<PublicKey?> LoadPublicKeyAsync();
    Task SavePublicKeyAsync(PublicKey publicKey);
}