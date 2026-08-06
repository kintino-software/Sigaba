using Sigaba.App.Services.Settings;
using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Services.Contexts;

internal class Context(IFileSystem fs, PrivateKey? privateKey, PublicKey? publicKey, IToolSettings toolSettings) : IContext
{
    PrivateKey? IContext.GetPrivateKey() => privateKey;

    PublicKey? IContext.GetPublicKey() => publicKey;

    bool IContext.FieldNameFilter(string fieldName) => toolSettings.FieldNamePredicate(fieldName);

    IEnumerable<string> IContext.GetWorkingSetFiles() => toolSettings.GetFilesWorkingSet(fs);
}

