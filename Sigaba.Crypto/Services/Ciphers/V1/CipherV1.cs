using Sigaba.Primitives.Crypto;

namespace Sigaba.Crypto.Services.Ciphers.V1;

internal class CipherV1 : IVersionedCipher
{
    byte IVersionedCipher.Version { get; } = 1;

    (PublicKey, PrivateKey) ICipher.GenerateKeys()
    {
        return AsymmetricAlgoV1.GenerateKeys();
    }

    EncryptedData ICipher.EncryptWithKey(PlainData plainData, PublicKey publicKey)
    {
        try
        {
            return AsymmetricAlgoV1.Encrypt(plainData, publicKey);
        }
        catch (Exception ex) when (ex is FormatException)
        {
            throw new InvalidOperationException("Invalid public key.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Encryption failed.", ex);
        }
    }

    PlainData ICipher.DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey)
    {
        return AsymmetricAlgoV1.Decrypt(encryptedData, privateKey);
    }

    EncryptedData ICipher.EncryptWithPassword(PlainData plainData, string password)
    {
        return PasswordAlgoV1.Encrypt(plainData, password);
    }

    PlainData ICipher.DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        return PasswordAlgoV1.Decrypt(encryptedData, password);
    }
}
