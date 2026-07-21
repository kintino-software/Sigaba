namespace Kintino.CipherConf.Documents.Services.Json.Serialization;

public record Node
{
    public required string Key { get; init; }
    public required string RawValue { get; init; }

}

internal class CustomConverter
{
}
