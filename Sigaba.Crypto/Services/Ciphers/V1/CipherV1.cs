using Sigaba.Primitives;

namespace Sigaba.Crypto.Services.Ciphers.V1;

internal class CipherV1 : IVersionedCipher
{
    byte IVersionedCipher.Version { get; } = 1;

    (PublicKey, PrivateKey) ICipher.GenerateKeys()
    {
        return AsymmetricAlgoV1.GenerateKeys();
    }

    EncryptedData ICipher.EncryptWithKey(IPlainData plainData, PublicKey publicKey)
    {
        return AsymmetricAlgoV1.Encrypt(plainData, publicKey);
    }

    PlainData ICipher.DecryptWithKey(IEncryptedData encryptedData, PrivateKey privateKey)
    {
        return AsymmetricAlgoV1.Decrypt(encryptedData, privateKey);
    }

    EncryptedData ICipher.EncryptWithPassword(IPlainData plainData, string password)
    {
        return PasswordAlgoV1.Encrypt(plainData, password);
    }

    PlainData ICipher.DecryptWithPassword(IEncryptedData encryptedData, string password)
    {
        return PasswordAlgoV1.Decrypt(encryptedData, password);
    }
}
