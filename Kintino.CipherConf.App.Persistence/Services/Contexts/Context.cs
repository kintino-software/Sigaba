using Kintino.CipherConf.App.Services.Settings;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.Contexts;

internal class Context(PrivateKey? privateKey, PublicKey? publicKey, IToolSettings toolSettings) : IContext
{
    PrivateKey? IContext.GetPrivateKey() => privateKey;

    PublicKey? IContext.GetPublicKey() => publicKey;

    bool IContext.FieldNameFilter(string fieldName) => toolSettings.FieldFilter(fieldName);

    IEnumerable<string> IContext.GetWorkingSetFiles() => toolSettings.WorkingSetFiles;
}

