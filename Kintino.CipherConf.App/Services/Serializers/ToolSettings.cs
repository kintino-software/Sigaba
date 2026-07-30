namespace Kintino.CipherConf.App.Services.Serializers;

public interface IToolSettings
{
    int Version { get; }
}

public record ToolSettingsV1 : IToolSettings
{
    public int Version { get; } = 1;
    public required string FieldRegex { get; init; }
    public required string[] IncludeFileGlob { get; init; }
    public required string[] ExcludeFileGlob { get; init; }
}
