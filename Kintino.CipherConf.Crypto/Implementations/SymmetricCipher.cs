using Kintino.CipherConf.Crypto.Services.Algos;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto.Implementations;

internal class SymmetricCipher : ISymmetricCipher
{
    PlainData ISymmetricCipher.Decrypt(PlainKey plainKey, EncryptedData encryptedData, Nonce nonce)
    {
        var plainBytes = AESCipher.Decrypt(
            encryptedData: encryptedData.Bytes,
            key: plainKey.Bytes,
            nonce: nonce.Bytes);
        return new PlainData(plainBytes);
    }

    EncryptedData ISymmetricCipher.Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce)
    {
        var cipherBytes = AESCipher.Encrypt(
            plainData: plainData.Bytes,
            key: plainKey.Bytes,
            nonce: nonce.Bytes);
        return new EncryptedData(cipherBytes);
    }
}
