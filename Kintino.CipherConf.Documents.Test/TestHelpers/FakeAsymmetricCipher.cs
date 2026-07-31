using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.TestHelpers;

internal class FakeAsymmetricCipher : IAsymmetricCipher
{
    public PrivateKey CorrectPrivateKey { get; } = new([2, 4, 6, 8]);
    public PublicKey CorrectPublicKey { get; } = new([1, 3, 5, 7]);

    public (PublicKey PublicKey, PrivateKey PrivateKey) CreateNewKeyPair()
    {
        return (CorrectPublicKey, CorrectPrivateKey);
    }

    public PlainKey Decrypt(EncryptedKey encryptedData, PrivateKey privateKey)
    {
        if (!privateKey.Bytes.SequenceEqual(CorrectPrivateKey.Bytes))
            throw new Exception("Wrong private key!");

        return new PlainKey([.. encryptedData.Bytes.Reverse()]);
    }

    public EncryptedKey Encrypt(PlainKey plainData, PublicKey publicKey)
    {
        if (!publicKey.Bytes.SequenceEqual(CorrectPublicKey.Bytes))
        {
            throw new Exception("Wrong public key!");
        }
        return new EncryptedKey([.. plainData.Bytes.Reverse()]);
    }
}