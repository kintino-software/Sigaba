using Kintino.CipherConf.App.Dependencies;
using Kintino.CipherConf.App.Primitives;
using Kintino.CipherConf.Crypto.Services.Algos;

namespace Kintino.CipherConf.Crypto.Services;

internal class SymmetricCipher : ISymmetricCipher
{
    PlainBytes ISymmetricCipher.Decrypt(PlainKey plainKey, CryptoBytes encryptedData, Nonce nonce)
    {
        var plainBytes = AESCipher.Decrypt(
            encryptedData: encryptedData.Bytes,
            key: plainKey.Bytes,
            nonce: nonce.Bytes);
        return new PlainBytes(plainBytes);
    }

    CryptoBytes ISymmetricCipher.Encrypt(PlainKey plainKey, PlainBytes plainData, Nonce nonce)
    {
        var cipherBytes = AESCipher.Encrypt(
            plainData: plainData.Bytes,
            key: plainKey.Bytes,
            nonce: nonce.Bytes);
        return new CryptoBytes(cipherBytes);
    }
}
