namespace Kintino.CipherConf.Documents.Primitives;

/// <summary>
/// Represents a raw value in a document, such as a property value in JSON or an element value in XML.
/// Example: in a json document, the raw values in {"name": "John", "age", "18" } is "\"John\"" and "18".
/// </summary>
internal record FieldRawValue
{
    public string Value { get; }

    public FieldRawValue(string value)
    {
        // as we do a lot of parsing and transforming, we want to ensure that the value is not null to avoid unexpected errors later on.
        // in a document, a raw value can be an empty string, but it should never be null.
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }

    public static implicit operator FieldRawValue(string value) => new(value);
    public static implicit operator string(FieldRawValue rawValue) => rawValue.Value;
}
