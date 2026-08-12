using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyLocationResolver
{
    FilePath GetSavePath(Guid projectId, DirPath? customLocation);
    FilePath GetLoadPath(Guid projectId, DirPath? customLocation);

}