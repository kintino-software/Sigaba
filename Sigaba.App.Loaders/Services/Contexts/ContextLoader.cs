using Sigaba.App.Services.PrivateKeys;
using Sigaba.App.Services.PublicKeys;
using Sigaba.App.Services.Settings;
using Sigaba.Crypto;
using System.IO.Abstractions;

namespace Sigaba.App.Services.Contexts;

internal class ContextLoader(
    IToolSettingsManager toolSettingsRepository,
    IPublicKeyManager publicKeyRepository,
    IPrivateKeyManager privateKeyRepository,
    ICipher cipher,
    IFileSystem fs) : IContextLoader
{
    async Task IContextLoader.CreateContextAsync()
    {
        if (await toolSettingsRepository.ExistsAsync())
        {
            throw new InvalidOperationException("A context already exists in this folder.");
        }

        var (publicKey, privateKey) = cipher.GenerateKeys();

        await toolSettingsRepository.SaveDefaultAsync();
        await publicKeyRepository.SaveAsync(publicKey);
        await privateKeyRepository.SaveAsync(privateKey);
    }

    async Task<IContext?> IContextLoader.LoadContextAsync()
    {
        if (!await toolSettingsRepository.ExistsAsync())
        {
            throw new InvalidOperationException("No context in this folder. You have to initialize it first.");
        }

        var toolSettings = await toolSettingsRepository.LoadAsync();
        var privateKey = await privateKeyRepository.LoadAsync();
        var publicKey = await publicKeyRepository.LoadAsync();
        return new Context(fs, privateKey, publicKey, toolSettings);
    }


}
