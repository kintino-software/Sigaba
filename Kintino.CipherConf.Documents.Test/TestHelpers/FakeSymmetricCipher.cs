using Kintino.CipherConf.Crypto;
using Kintino.CipherConf.Documents.Adaptors;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Documents.TestHelpers;

public class FakeSymmetricCipher : ISymmetricCipher
{
    public virtual PlainData Decrypt(PlainKey cipherKey, EncryptedData encryptedData, Nonce nonce)
    {
        var base64 = encryptedData.Bytes.ToUTF8String();
        var splits = base64.Split('.');
        if (splits.Length != 3)
            throw new InvalidOperationException("Invalid encrypted data format.");
        var savedNonce = new Nonce(new(splits[0].FromBase64String()));
        if (!savedNonce.Bytes.SequenceEqual(nonce.Bytes)) throw new Exception("Wrong nonce!");

        var savedKey = new PlainKey(new(splits[2].FromBase64String()));
        if (!savedKey.Bytes.SequenceEqual(cipherKey.Bytes)) throw new Exception("Wrong key!");

        var savedPlainData = new PlainData(splits[1].FromBase64String());
        return savedPlainData;
    }

    public virtual EncryptedData Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce)
    {
        var base64 = $"{nonce.Bytes.ToBase64String()}.{plainData.Bytes.ToBase64String()}.{plainKey.Bytes.ToBase64String()}";
        return new EncryptedData(base64.ToUTF8Bytes());
    }
}
