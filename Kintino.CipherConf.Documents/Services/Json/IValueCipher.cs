using Kintino.CipherConf.App.Primitives;
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
    /// <param name="originalEncryptedJsonValue">The JsonValue to decrypt.</param>
    /// <param name="key">The key to use for decryption.</param>
    /// <returns>A new JsonValue that is the decrypted version of the provided jsonNode.</returns>
    JsonNode? CreateDecryptedValue(JsonNode originalEncryptedJsonValue, PlainKey key);
    /// <summary>
    /// Creates a new JsonValue that is the encrypted version of the provided jsonValue using the specified key.
    /// </summary>
    /// <param name="originalPlainJsonValue">The JsonValue to encrypt.</param>
    /// <param name="key">The key to use for encryption.</param>
    /// <returns>A new JsonValue that is the encrypted version of the provided jsonValue.</returns>
    JsonNode? CreateEncryptedValue(JsonNode originalPlainJsonValue, PlainKey key);
    /// <summary>
    /// Determines whether the specified JsonValue is encrypted.
    /// Implementations should create ways to identify if a JsonValue is encrypted, such as checking for specific markers or patterns in the value.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to check.</param>
    /// <returns>True if the JsonValue is encrypted; otherwise, false.</returns>
    bool IsEncrypted(JsonNode? jsonValue);
}
