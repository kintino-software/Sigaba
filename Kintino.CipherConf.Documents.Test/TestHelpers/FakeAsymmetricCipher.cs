using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.TestHelpers;

internal class FakeAsymmetricCipher : IAsymmetricCipher
{
    public PrivateKey CorrectPrivateKey { get; } = new(new byte[] { 1, 2, 3, 4 });
    public PublicKey CorrectPublicKey { get; } = new(new byte[] { 1, 2, 3, 4 });

    public virtual (PublicKey PublicKey, PrivateKey PrivateKey) CreateNewKeyPair()
    {
        return (CorrectPublicKey, CorrectPrivateKey);
    }

    public virtual PlainData Decrypt(EncryptedData encryptedData, PrivateKey privateKey)
    {
        if (!privateKey.Bytes.SequenceEqual(CorrectPrivateKey.Bytes))
        {
            throw new Exception("Wrong private key!");

        }

        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }

    public virtual EncryptedData Encrypt(PlainData plainData, PublicKey publicKey)
    {
        if (!publicKey.Bytes.SequenceEqual(CorrectPublicKey.Bytes))
        {
            throw new Exception("Wrong public key!");
        }
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }
}