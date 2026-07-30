using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.Crypto;

/// <summary>
/// Represents a symmetric cipher that can encrypt and decrypt data using the same key for both operations.
/// </summary>
public interface ISymmetricCipher
{
    int Version { get; }
    PlainKey GenerateNewKey();
    Nonce GenerateNewNonce();
    EncryptedData Encrypt(PlainKey plainKey, PlainData plainData, Nonce nonce);
    PlainData Decrypt(PlainKey plainKey, EncryptedData encryptedData, Nonce nonce);
}
