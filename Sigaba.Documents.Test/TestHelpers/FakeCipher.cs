using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.Documents.TestHelpers;

public class FakeCipher : ICipher
{
    public PrivateKey ThePrivateKey { get; } = new([9, 9, 9, 9, 9]);
    public PublicKey ThePublicKey { get; } = new([8, 8, 8, 8, 8,]);
    private bool checkKeysAndPassword = false;

    public FakeCipher CheckKeysAndPasswords(bool checkKeysAndPassword)
    {
        this.checkKeysAndPassword = checkKeysAndPassword;
        return this;
    }

    public (PublicKey, PrivateKey) GenerateKeys()
    {
        return (ThePublicKey, ThePrivateKey);
    }

    public EncryptedData EncryptWithKey(PlainData plainData, PublicKey publicKey)
    {
        if (checkKeysAndPassword && !publicKey.Bytes.SequenceEqual(ThePublicKey.Bytes))
            throw new Exception("Wrong public key!");
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public PlainData DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey)
    {
        if (checkKeysAndPassword && !privateKey.Bytes.SequenceEqual(ThePrivateKey.Bytes))
            throw new Exception("Wrong private key!");
        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }

    public EncryptedData EncryptWithPassword(PlainData plainData, string password)
    {
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public PlainData DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        if (checkKeysAndPassword && password != "password")
            throw new Exception("Wrong password! For this fake, use 'password' as password.");

        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }
}