using Sigaba.App.Services.Settings;
using Sigaba.Primitives;
using System.IO.Abstractions;

namespace Sigaba.App.Models;

internal class Context(string projectRoot, PrivateKey? privateKey, PublicKey? publicKey, IToolSettings toolSettings, IFileSystem fs) : IContext
{
    PrivateKey? IContext.GetPrivateKey() => privateKey;

    PublicKey? IContext.GetPublicKey() => publicKey;

    bool IContext.FieldNameFilter(string fieldName) => toolSettings.FieldNamePredicate(fieldName);

    IEnumerable<string> IContext.GetWorkingSetFiles() => toolSettings.GetFilesWorkingSet(fs, projectRoot);
}

