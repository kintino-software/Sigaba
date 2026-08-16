using System.Diagnostics.CodeAnalysis;

namespace Sigaba.Documents.Models;
/// <summary>
/// Represents a document model that can be parsed, serialilzed and offers functionality to query its fields.
/// </summary>
internal interface IDocumentModel
{
    /// <summary>
    /// Parses the document content and populates the model's fields accordingly.
    /// </summary>
    /// <param name="documentContent">The content of the document to parse.</param>
    void Parse(string documentContent);

    /// <summary>
    /// Serializes the model's fields into a string representation of the document.
    /// </summary>
    /// <returns>The string representation of the document.</returns>
    string Serialize();

    /// <summary>
    /// Gets the names of all fields in the model.
    /// </summary>
    /// <returns>An enumerable of field names.</returns>
    IEnumerable<string> GetFieldNames();

    /// <summary>
    /// Tries to get the value of a field with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the field value.</typeparam>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="value">When this method returns, contains the value of the field if found; otherwise, the default value for the type.</param>
    /// <returns>true if the field was found; otherwise, false.</returns>
    bool TryGetValue<T>(string fieldName, [MaybeNull] out T value);

    /// <summary>
    /// Sets the value of a field with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the field value.</typeparam>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="value">The value to set.</param>
    void SetFieldValue<T>(string fieldName, [MaybeNull] T value);

    /// <summary>
    /// Gets the raw string value of a field with the specified key.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <returns>The raw string value of the field.</returns>
    string GetFieldRawValue(string fieldName);

    /// <summary>
    /// Sets the raw string value of a field with the specified key.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="rawValue">The raw string value to set.</param>
    void SetFieldRawValue(string fieldName, string rawValue);

}
