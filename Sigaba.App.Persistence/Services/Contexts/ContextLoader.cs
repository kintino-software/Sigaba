using Sigaba.App.Services.Common;
using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.PublicKeys;
using Sigaba.App.Services.Settings;
using Sigaba.Crypto;
using System.IO.Abstractions;

namespace Sigaba.App.Services.Contexts;

internal class ContextLoader(
    IToolSettingsRepository toolSettingsRepository,
    IPublicKeyRepository publicKeyRepository,
    IPrivateKeyRepository privateKeyRepository,
    IAsymmetricCipher asymmetricCipher,
    IFileSystem fs) : IContextLoader
{
    private readonly ToolEnvironment environment = new(fs);

    // IContextLoader implementation

    async Task IContextLoader.CreateContextAsync(string projectRootFolder)
    {
        ResolveFilePaths(projectRootFolder, out var settingsFilePath, out var publicKeyFilePath, out var privateKeyFilePath);
        if (fs.File.Exists(settingsFilePath))
        {
            throw new InvalidOperationException("A context already exists in this folder.");
        }

        var (publicKey, privateKey) = asymmetricCipher.CreateNewKeyPair();
        await toolSettingsRepository.SaveDefaultAsync(settingsFilePath);
        await publicKeyRepository.SaveAsync(publicKey, publicKeyFilePath);
        await privateKeyRepository.SaveAsync(privateKey, privateKeyFilePath);
    }

    async Task<IContext?> IContextLoader.LoadContextAsync(string currentFolder)
    {
        ResolveFilePaths(currentFolder, out var settingsFilePath, out var publicKeyFilePath, out var privateKeyFilePath);

        var toolSettings = await toolSettingsRepository.LoadAsync(settingsFilePath)
            ?? throw new InvalidOperationException("No context in this folder. You have to initialize it first.");
        var privateKey = await privateKeyRepository.LoadAsync(privateKeyFilePath);
        var publicKey = await publicKeyRepository.LoadAsync(publicKeyFilePath);
        return new Context(currentFolder, privateKey, publicKey, toolSettings, fs);
    }

    // helpers

    private static void ResolveFilePaths(string targetFolder, out string settingsFilePath, out string publicKeyFilePath, out string privateKeyFilePath)
    {
        settingsFilePath = Path.Combine(targetFolder, Constants.ToolSettingsFileName);
        publicKeyFilePath = Path.Combine(targetFolder, Constants.PublicKeyFileName);
        privateKeyFilePath = Path.Combine(targetFolder, Constants.PrivateKeyFileName);
    }

}
