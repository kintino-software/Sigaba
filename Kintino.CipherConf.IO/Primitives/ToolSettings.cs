namespace Kintino.CipherConf.IO.Primitives;

internal class ToolSettings : ISerializable
{
    public required string? PropertyRegex { get; init; }
    public required string? FileRegex { get; init; }
    public required string Key { get; init; }
}
