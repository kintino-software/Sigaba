using Kintino.CipherConf.IO.Implementations;

namespace Kintino.CipherConf.IO.Services;

internal interface IContextSerializer
{
    Task<Context> DeserializeFromFileSystem(string settingsFilePath, string privateKeyFilePath, string publicKeyFilePath);
    Task SerializeToFileSystem(Context context, string settingsFilePath, string privateKeyFilePath, string publicKeyFilePath);
}