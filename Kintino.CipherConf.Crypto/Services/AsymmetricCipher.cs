using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.Crypto.Services.Algos;

namespace Kintino.CipherConf.Crypto.Services;

internal class AsymmetricCipher : IAsymmetricCipher
{
    void IAsymmetricCipher.CreateNewKeyPair(out PublicKey publicKey, out PrivateKey privateKey)
    {
        RSACipher.CreateNewKeyPair(out var pubKey, out var privKey);
        publicKey = new PublicKey(pubKey);
        privateKey = new PrivateKey(privKey);
    }

    PlainBytes IAsymmetricCipher.Decrypt(CryptoBytes cryptoBytes, PrivateKey privateKey)
    {
        var bytes = RSACipher.Decrypt(cryptoBytes.Bytes, privateKey.Bytes);
        return new PlainBytes(bytes);
    }

    CryptoBytes IAsymmetricCipher.Encrypt(PlainBytes plainBytes, PublicKey publicKey)
    {
        var encryptedBytes = RSACipher.Encrypt(plainBytes.Bytes, publicKey.Bytes);
        return new CryptoBytes(encryptedBytes);
    }
}
