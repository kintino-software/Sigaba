namespace Kintino.CipherConf.Documents.Primitives;

/// <summary>
/// Represents a key in a document, such as a property name in JSON or an element name in XML.
/// </summary>
/// <param name="Value">The value of the key.</param>
internal record FieldKey
{
    public string Value { get; }

    public FieldKey(string value)
    {
        // As we do a lot of parsing and transforming, we want to ensure that the value is not null to avoid unexpected errors later on.
        // In a document, a field key can be an empty string, but it should never be null.
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }

    public static implicit operator FieldKey(string value) => new(value);
    public static implicit operator string(FieldKey fieldKey) => fieldKey.Value;
}
