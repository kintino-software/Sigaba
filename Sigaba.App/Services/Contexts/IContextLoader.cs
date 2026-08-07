using Sigaba.Primitives;

namespace Sigaba.App.Services.Contexts;

internal interface IContextLoader
{
    Task<Context> LoadContextFromFolderAsync(string folderPath);
    Task CreateContextAsync(string initializationFolderPath, PublicKey publicKey, PrivateKey privateKey);
}
