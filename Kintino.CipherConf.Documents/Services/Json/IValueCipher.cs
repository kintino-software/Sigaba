using Kintino.CipherConf.Primitives;
using System.Text.Json.Nodes;

namespace Kintino.CipherConf.Documents.Services.Json;

/// <summary>
/// Represents a cipher for encrypting and decrypting JSON values.
/// <br/>
/// Methods here return new JsonValues based on original ones, as JsonValues are immutable.
/// <br/>
/// The caller is responsible for replacing the original JsonValue with the new one in the JSON structure.
/// </summary>
public interface IValueCipher
{
    /// <summary>
    /// Creates a new JsonValue that is the decrypted version of the provided jsonNode using the specified key.
    /// </summary>
    /// <param name="encryptedNode">The json node to decrypt.</param>
    /// <param name="key">The key to use for decryption.</param>
    /// <returns>A new JsonValue that is the decrypted version of the provided jsonNode.</returns>
    JsonNode? CreateDecryptedValue(JsonNode encryptedNode, PlainKey key);
    /// <summary>
    /// Creates a new JsonValue that is the encrypted version of the provided jsonValue using the specified key.
    /// </summary>
    /// <param name="plainNode">The json node to encrypt.</param>
    /// <param name="key">The key to use for encryption.</param>
    /// <returns>A new JsonValue that is the encrypted version of the provided jsonValue.</returns>
    JsonNode? CreateEncryptedValue(JsonNode plainNode, PlainKey key);
}
