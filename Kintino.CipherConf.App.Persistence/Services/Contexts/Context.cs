using Kintino.CipherConf.App.Services.Settings;
using Kintino.CipherConf.Primitives;

namespace Kintino.CipherConf.App.Services.Contexts;

internal class Context(string projectRoot, PrivateKey? privateKey, PublicKey? publicKey, IToolSettings toolSettings) : IContext
{
    PrivateKey? IContext.GetPrivateKey() => privateKey;

    PublicKey? IContext.GetPublicKey() => publicKey;

    bool IContext.FieldNameFilter(string fieldName) => toolSettings.FieldNamePredicate(fieldName);

    IEnumerable<string> IContext.GetWorkingSetFiles() => toolSettings.GetFilesWorkingSet(projectRoot);
}

