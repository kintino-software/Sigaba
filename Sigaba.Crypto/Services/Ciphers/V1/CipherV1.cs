using Sigaba.Primitives.Crypto;
using System.Security.Cryptography;

namespace Sigaba.Crypto.Services.Ciphers.V1;

internal partial class CipherV1
{
    private static T TryExecute<T>(Func<T> action, string errorMessage)
    {
        try
        {
            return action();
        }
        catch (Exception ex) when (ex is FormatException || ex is CryptographicException)
        {
            throw new InvalidOperationException(errorMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(errorMessage, ex);
        }
    }
}

internal partial class CipherV1 : IVersionedCipher
{
    byte IVersionedCipher.Version { get; } = 1;

    (PublicKey, PrivateKey) ICipher.GenerateKeys()
    {
        return AsymmetricAlgoV1.GenerateKeys();
    }

    EncryptedData ICipher.EncryptWithKey(PlainData plainData, PublicKey publicKey)
    {
        return TryExecute(() => AsymmetricAlgoV1.Encrypt(plainData, publicKey), "Encryption failed.");
    }

    PlainData ICipher.DecryptWithKey(EncryptedData encryptedData, PrivateKey privateKey)
    {
        return TryExecute(() => AsymmetricAlgoV1.Decrypt(encryptedData, privateKey), "Decryption failed.");
    }

    EncryptedData ICipher.EncryptWithPassword(PlainData plainData, string password)
    {
        return TryExecute(() => PasswordAlgoV1.Encrypt(plainData, password), "Encryption with password failed.");
    }

    PlainData ICipher.DecryptWithPassword(EncryptedData encryptedData, string password)
    {
        return TryExecute(() => PasswordAlgoV1.Decrypt(encryptedData, password), "Decryption with password failed.");
    }
}
