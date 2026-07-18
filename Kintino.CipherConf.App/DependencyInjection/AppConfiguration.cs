using Kintino.CipherConf.IO.Dependencies;

namespace Kintino.CipherConf.App.DependencyInjection;

public partial class AppConfiguration : IIOConfiguration
{
    public required string PrivateKeyFileName { get; init; }
    public required string PublicKeyFileName { get; init; }
    public required string ToolSettingsFileName { get; init; }
}
