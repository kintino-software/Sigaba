using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.Documents.TestHelpers;

internal class FakeCipher : ICipher
{
    public PrivateKey CorrectPrivateKey { get; } = new([2, 4, 6, 8]);
    public PublicKey CorrectPublicKey { get; } = new([1, 3, 5, 7]);
    public string CorrectPassword { get; } = "correct_password";

    public (PublicKey, PrivateKey) GenerateKeys()
    {
        return (CorrectPublicKey, CorrectPrivateKey);
    }

    public PlainData DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey)
    {
        if (!privateKey.Bytes.SequenceEqual(CorrectPrivateKey.Bytes))
            throw new Exception("Wrong private key!");

        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }


    public EncryptedData EncryptWithKey(PlainData plainData, PublicKey publicKey)
    {
        if (!publicKey.Bytes.SequenceEqual(CorrectPublicKey.Bytes))
            throw new Exception("Wrong public key!");
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public EncryptedData EncryptWithPassword(PlainData plainData, string password)
    {
        if (password != CorrectPassword)
            throw new Exception("Wrong password!");

        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public PlainData DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        if (password != CorrectPassword)
            throw new Exception("Wrong password!");

        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }
}