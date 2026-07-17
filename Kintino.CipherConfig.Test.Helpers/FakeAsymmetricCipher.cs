using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConfig;

public class FakeAsymmetricCipher : IAsymmetricCipher
{
    public PublicKey PublicKey { get; } = new(new([1, 2, 3, 4]));
    public PrivateKey PrivateKey { get; } = new(new([5, 6, 7, 8]));
    private bool shouldThrow = false;

    public (PublicKey PublicKey, PrivateKey PrivateKey) CreateNewKeyPair()
    {
        return (PublicKey, PrivateKey);
    }

    public PlainData Decrypt(EncryptedData encryptedData, PrivateKey privateKey)
    {
        if (shouldThrow)
        {
            throw new InvalidOperationException("Decryption failed.");
        }
        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }

    public EncryptedData Encrypt(PlainData plainData, PublicKey publicKey)
    {
        if (shouldThrow)
        {
            throw new InvalidOperationException("Encryption failed.");
        }
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public FakeAsymmetricCipher MakeItThrow(bool shouldThrow = false)
    {
        this.shouldThrow = shouldThrow;
        return this;
    }
}
