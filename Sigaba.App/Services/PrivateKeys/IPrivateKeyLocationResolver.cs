namespace Sigaba.App.Services.PrivateKeys;

internal interface IPrivateKeyLocationResolver
{
    string ResolveCurrentLocation(Guid projectId);

    string GetDefaultFilePath(Guid projectId);
}