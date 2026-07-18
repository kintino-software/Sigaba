using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Models;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services;

internal class Facade(IAsymmetricCipher asymmetricCipher, IRandomKeyGenerator randomKeyGenerator) : IFacade
{
    (PublicKey, PrivateKey, EncryptedKey) IFacade.CreateContextKeys()
    {
        var (publicKey, privateKey) = asymmetricCipher.CreateNewKeyPair();
        var plainKey = randomKeyGenerator.GenerateNewKey();
        var encryptedData = asymmetricCipher.Encrypt(new PlainData(plainKey.Bytes), publicKey);
        return (publicKey, privateKey, new EncryptedKey(encryptedData));
    }

    PlainKey IFacade.DecryptKeyFromContext(IContext context)
    {
        var encryptedData = new EncryptedData(context.Key.Bytes);
        var plainData = asymmetricCipher.Decrypt(encryptedData, context.PrivateKey);
        return new PlainKey(plainData);
    }

}
