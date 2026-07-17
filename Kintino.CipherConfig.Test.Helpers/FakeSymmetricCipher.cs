using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeSymmetricCipher : ISymmetricCipher
{
    private bool _shouldThrow = false;

    public PlainData Decrypt(PlainKey cipherKey, EncryptedData encryptedData, Nonce nonce)
    {
        if (_shouldThrow)
        {
            throw new InvalidOperationException("Decryption failed.");
        }
        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }

    public EncryptedData Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce)
    {
        if (_shouldThrow)
        {
            throw new InvalidOperationException("Encryption failed.");
        }
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public FakeSymmetricCipher MakeItThrow(bool shouldThrow = true)
    {
        _shouldThrow = shouldThrow;
        return this;
    }
}
