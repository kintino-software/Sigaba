using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.TestHelpers;

internal class FakeAsymmetricCipher : IAsymmetricCipher
{
    public PrivateKey CorrectPrivateKey { get; } = new([1, 2, 3, 4]);
    public PublicKey CorrectPublicKey { get; } = new([1, 2, 3, 4]);
    public int Version { get; } = 1;

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