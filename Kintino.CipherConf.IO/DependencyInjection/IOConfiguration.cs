using Kintino.CipherConf.IO.Dependencies;

namespace Kintino.CipherConf.IO.DependencyInjection;

public record IOConfiguration : IIOConfiguration
{
    public required string PrivateKeyFileName { get; init; }
    public required string PublicKeyFileName { get; init; }
    public required string ToolSettingsFileName { get; init; }
}
