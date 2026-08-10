using Sigaba.Primitives;

namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyLocationResolver
{
    FilePath ResolveCurrentLocation(Guid projectId);

    FilePath GetDefaultFilePath(Guid projectId);
}