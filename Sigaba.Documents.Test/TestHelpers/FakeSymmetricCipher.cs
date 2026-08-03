using Sigaba.Crypto;
using Sigaba.Primitives;

namespace Sigaba.Documents.TestHelpers;

public class FakeSymmetricCipher : ISymmetricCipher
{
    public PlainKey Key { get; } = new([1, 1, 1, 1]);
    public Nonce Nonce { get; } = new([2, 2, 2, 2]);

    public virtual PlainData Decrypt(PlainKey cipherKey, EncryptedData encryptedData, Nonce nonce)
    {
        if (!cipherKey.Bytes.SequenceEqual(Key.Bytes))
            throw new Exception("Wrong key!");
        if (!nonce.Bytes.SequenceEqual(Nonce.Bytes))
            throw new Exception("Wrong nonce!");
        return new PlainData([.. encryptedData.Bytes.Reverse()]);
    }

    public virtual EncryptedData Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce)
    {
        return new EncryptedData([.. plainData.Bytes.Reverse()]);
    }

    public PlainKey GenerateNewKey() => Key;

    public Nonce GenerateNewNonce() => Nonce;
}
