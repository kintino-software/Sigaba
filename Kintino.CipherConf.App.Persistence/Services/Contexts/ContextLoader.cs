using Kintino.CipherConf.App.Services.Common;
using Kintino.CipherConf.App.Services.PrivateKeys;
using Kintino.CipherConf.App.Services.PublicKeys;
using Kintino.CipherConf.App.Services.Settings;
using Kintino.CipherConf.Crypto;
using System.IO.Abstractions;

namespace Kintino.CipherConf.App.Services.Contexts;

internal class ContextLoader(
    IToolSettingsRepository toolSettingsRepository,
    IPublicKeyRepository publicKeyRepository,
    IPrivateKeyRepository privateKeyRepository,
    IAsymmetricCipher asymmetricCipher,
    IFileSystem fs) : IContextLoader
{
    private readonly ToolEnvironment environment = new(fs);

    async Task IContextLoader.CreateContextAsync(string projectRootFolder)
    {
        ResolveFilePaths(projectRootFolder, out var settingsFilePath, out var publicKeyFilePath, out var privateKeyFilePath);

        var (publicKey, privateKey) = asymmetricCipher.CreateNewKeyPair();
        await toolSettingsRepository.SaveDefaultAsync(settingsFilePath);
        await publicKeyRepository.SaveAsync(publicKey, publicKeyFilePath);
        await privateKeyRepository.SaveAsync(privateKey, privateKeyFilePath);
    }

    Task<bool> IContextLoader.HasContextAsync(string folderPath)
    {
        ResolveFilePaths(folderPath, out var settingsFilePath, out _, out _);
        return Task.FromResult(fs.File.Exists(settingsFilePath));
    }

    async Task<IContext?> IContextLoader.LoadContextAsync(string currentFolder)
    {
        ResolveFilePaths(currentFolder, out var settingsFilePath, out var publicKeyFilePath, out var privateKeyFilePath);

        var toolSettings = await toolSettingsRepository.LoadAsync(settingsFilePath)
            ?? throw new InvalidOperationException("No settings found.");
        var privateKey = await privateKeyRepository.LoadAsync(privateKeyFilePath);
        var publicKey = await publicKeyRepository.LoadAsync(publicKeyFilePath);
        return new Context(privateKey, publicKey, toolSettings);
    }

    private static void ResolveFilePaths(string targetFolder, out string settingsFilePath, out string publicKeyFilePath, out string privateKeyFilePath)
    {
        settingsFilePath = Path.Combine(targetFolder, Constants.ToolSettingsFileName);
        publicKeyFilePath = Path.Combine(targetFolder, Constants.PublicKeyFileName);
        privateKeyFilePath = Path.Combine(targetFolder, Constants.PrivateKeyFileName);
    }

}
