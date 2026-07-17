namespace Kintino.CipherConf.IO.Dependencies;

public interface IIOConfiguration
{
    string PrivateKeyFileName { get; }
    string PublicKeyFileName { get; }
    string ToolSettingsFileName { get; }
}
