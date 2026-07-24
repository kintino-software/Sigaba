using Kintino.CipherConf.IO.Dependencies;
using System.IO.Abstractions;

namespace Kintino.CipherConf.DependencyInjection;

interface IGlobalConfiguration
{
    public IFileSystem FileSystem { get; }
}

public partial class AppConfiguration : IGlobalConfiguration
{
    public IFileSystem FileSystem { get; set; } = new FileSystem();
}

public partial class AppConfiguration : IIOConfiguration
{
    public string PrivateKeyFileName { get; set; } = "private.key";
    public string PublicKeyFileName { get; set; } = "public.key";
    public string ToolSettingsFileName { get; set; } = "cipherconf.json";
}
