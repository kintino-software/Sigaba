using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Models;
using Kintino.CipherConf.App.Primitives;

namespace Kintino.CipherConf.App.Services;

internal class Facade(IAsymmetricCipher asymmetricCipher, IRandomKeyGenerator randomKeyGenerator) : IFacade
{
    PlainKey IFacade.DecryptKeyFromContext(Context context)
    {
        var cryptoBytes = new CryptoBytes(context.Key.Bytes);
        var plainBytes = asymmetricCipher.Decrypt(cryptoBytes, context.PrivateKey);
        return new PlainKey(plainBytes.Bytes);
    }

    (PublicKey, PrivateKey, CryptoKey) IFacade.CreateContextKeys()
    {
        asymmetricCipher.CreateNewKeyPair(out var publicKey, out var privateKey);
        var plainKey = randomKeyGenerator.GenerateNewKey();
        var plainBytes = new PlainBytes(plainKey.Bytes);
        var cryptoBytes = asymmetricCipher.Encrypt(plainBytes, publicKey);
        var cryptoKey = new CryptoKey(cryptoBytes.Bytes);
        return (publicKey, privateKey, cryptoKey);
    }
}
