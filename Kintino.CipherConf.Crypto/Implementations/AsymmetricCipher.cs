using Kintino.CipherConf.Crypto.Services.Algos;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto.Implementations;

internal class AsymmetricCipher : IAsymmetricCipher
{
    (PublicKey PublicKey, PrivateKey PrivateKey) IAsymmetricCipher.CreateNewKeyPair()
    {
        RSACipher.CreateNewKeyPair(out var pubKey, out var privKey);
        var publicKey = new PublicKey(new PlainData(pubKey));
        var privateKey = new PrivateKey(new PlainData(privKey));
        return (publicKey, privateKey);
    }

    PlainData IAsymmetricCipher.Decrypt(EncryptedData encryptedData, PrivateKey privateKey)
    {
        var bytes = RSACipher.Decrypt(encryptedData.Bytes, privateKey.Bytes);
        return new PlainData(bytes);
    }

    EncryptedData IAsymmetricCipher.Encrypt(PlainData plainData, PublicKey publicKey)
    {
        var encryptedBytes = RSACipher.Encrypt(plainData.Bytes, publicKey.Bytes);
        return new EncryptedData(encryptedBytes);
    }
}
